using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Pages;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.Test.ParseHandling.Pages
{
    [TestClass]
    public class PageTest
    {
        private struct Patterns
        {
            public const string date = @"^((0[1-9]|[12]\d|3[01])-(0[1-9]|1[0-2])-[12]\d{3} 00:00:00)$";
            public const string iso = @"^[a-zA-Z]{3}$";
            public const string year = @"^[0-9]{4}$";
            public const string link = @"(^http.+)|(\/)";
            public const string captain = @"(^[01]$)";
            public const string height = @"(^[12][0-9]{2}$)";
            public const string foot = @"^([R|L|A]{1})$";
            public const string position = @"^[a-zA-Z]{2}$";
            public const string mv = @"[0-9]+";
            public const string shirt = @"[0-9]+";
            public const string name = @"^(\D)*$";
            public const string abbrevName = @"^(\D)*$";
        }

        //TODO: consider create enum to hold and retrieve config settings and a generic GetAppSettings<T> to retrive it as a type.
        private static IConfigurationManager config = new ConfigManager();

        private static string MinimumLoggingLevel { get; } = config.GetAppSetting("MinimumLoggingLevel");
        private static string LogPath { get; } = config.GetAppSetting("LogPath");

        private static readonly IDictionary<Type, string> PatternsMap = new Dictionary<Type, string> {
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Competition.Country), Patterns.iso },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Competition.Name), Patterns.name },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Competition.Season), Patterns.year },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Competition.ImgUrl), Patterns.link },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Competition.CountryImg), Patterns.link },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Club.Country), Patterns.iso },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Club.Name), Patterns.name },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Club.Season), Patterns.year },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Club.ImgUrl), Patterns.link },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Club.CountryImg), Patterns.link },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.Name), Patterns.name },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.ShortName), Patterns.abbrevName },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.BirthDate), Patterns.date },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.Nationality), Patterns.iso },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.Height), Patterns.height },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.PreferredFoot), Patterns.foot },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.Position), Patterns.position },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.ShirtNumber), Patterns.shirt },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.Captain), Patterns.captain },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.ClubArrivalDate), Patterns.date },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.ContractExpirationDate), Patterns.date },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.MarketValue), Patterns.mv },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.ImgUrl), Patterns.link },
            { typeof(Transfermarkt.Core.ParseHandling.Elements.Player.ProfileUrl), Patterns.link }
        };

        private static readonly IDictionary<string, (int id, string internalName)> clubs = new Dictionary<string, (int, string)>
        {
            ["Barcelona"] = (131, "fc-barcelona"),
            ["Nacional"] = (982, "cd-nacional"),
            ["V. Guimarães"] = (2420, "vitoria-sc"),
        };

        private static IDictionary<Nationality, (string internalName, string d1, string d2)> competitions = new Dictionary<Nationality, (string internalName, string d1, string d2)>
        {
            [Nationality.ITA] = ("serie-a", "IT1", "")
        };

        private static readonly int currentSeason = (DateTime.Today.Year < 8) ? DateTime.Today.Year - 1 : DateTime.Today.Year;

        private static readonly ILogger logger = LoggerFactory.GetLogger(LogPath, int.Parse(MinimumLoggingLevel));


        [TestMethod, TestCategory("Page Parsing")]
        public void TestClubParsing()
        {
            string url = "https://www.transfermarkt.pt/fc-barcelona/kader/verein/131/plus/1/galerie/0?saison_id=2011";
            ClubPage page = new ClubPage(new HAPConnection(), logger);
            page.Parse(url);

            var domain = page.Domain;
            Assert.IsNotNull(domain, "The returned Domain is null.");

            DomainElementsCheck(domain);
            for (int i = 0; i < domain.Children.Count; i++)
            {
                DomainElementsCheck(domain.Children[i]);
            }
        }

        [TestMethod, TestCategory("Page Parsing")]
        public void TestCompetitionParsing()
        {
            string url = "https://www.transfermarkt.pt/serie-a/startseite/wettbewerb/IT1";
            url = "https://www.transfermarkt.pt/liga-nos/startseite/wettbewerb/PO1/plus/?saison_id=2019";
            CompetitionPage page = new CompetitionPage(new HAPConnection(), logger);
            page.Parse(url);

            var domain = page.Domain;
            Assert.IsNotNull(domain, "The returned Domain is null.");

            DomainElementsCheck(domain);

            // Clubs
            for (int i = 0; i < domain.Children.Count; i++)
            {
                var clubChild = domain.Children[i];
                DomainElementsCheck(clubChild);

                for (int j = 0; j < clubChild.Children.Count; j++)
                {
                    var playerChild = clubChild.Children[j];
                    DomainElementsCheck(playerChild);
                }
            }
        }

        [TestMethod, TestCategory("Page Parsing")]
        public void TestContinentParsing()
        {
            string[] urls = new string[]
            {
                "https://www.transfermarkt.pt/wettbewerbe/europa",
                "https://www.transfermarkt.pt/wettbewerbe/amerika",
                "https://www.transfermarkt.pt/wettbewerbe/asien",
                "https://www.transfermarkt.pt/wettbewerbe/afrika"
            };
            var append = "/wettbewerbe?plus=1";

            foreach (var url in urls)
            {
                //ContinentPage page = new ContinentPage(url);
                //page.Parse();
            }
        }

        private void DomainElementsCheck(IDomain<IValue> domain)
        {
            for (int i = 0; i < domain.Elements.Count; i++)
            {
                string value = null;
                var e = domain.Elements[i];
                if (e.Value.Type == typeof(string))
                {
                    value = ((StringValue)e.Value).Value;
                }
                else if (e.Value.Type == typeof(int?))
                {
                    value = ((IntValue)e.Value).Value?.ToString();
                }
                else if (e.Value.Type == typeof(decimal?))
                {
                    value = ((DecimalValue)e.Value).Value?.ToString();
                }
                else if (e.Value.Type == typeof(DateTime?))
                {
                    value = ((DatetimeValue)e.Value).Value?.ToString();
                }
                else if (e.Value.Type == typeof(Nationality?))
                {
                    value = ((NationalityValue)e.Value).Value?.ToString();
                }
                else if (e.Value.Type == typeof(Position?))
                {
                    value = ((PositionValue)e.Value).Value?.ToString();
                }
                else if (e.Value.Type == typeof(Foot?))
                {
                    value = ((FootValue)e.Value).Value?.ToString();
                }

                if (value != null)
                {
                    var pattern = PatternsMap[e.GetType()];
                    Regex rgx = new Regex(pattern);
                    var str = string.Format("{0}", value);
                    var m = rgx.Match(str);
                    Assert.IsTrue(rgx.IsMatch(str), $"{e.InternalName} returned an unexpected value: {value}");
                }
            }
        }
    }
}
