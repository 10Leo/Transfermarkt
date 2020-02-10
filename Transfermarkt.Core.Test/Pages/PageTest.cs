using System;
using System.Collections.Generic;
using System.Linq;
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
        //consider create enum to retrieve configs and a generic GetAppSettings<T> to retrive it as a type
        private static IConfigurationManager config = new ConfigManager();

        private static string BaseURL { get; } = config.GetAppSetting("BaseURL");
        private static string PlusClubUrlFormat { get; } = config.GetAppSetting("PlusClubUrlFormatV2");
        private static string CompetitionUrlFormat { get; } = config.GetAppSetting("CompetitionUrlFormat");
        private static string MinimumLoggingLevel { get; } = config.GetAppSetting("MinimumLoggingLevel");
        private static string LogPath { get; } = config.GetAppSetting("LogPath");

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

        private static int currentSeason = (DateTime.Today.Year < 8) ? DateTime.Today.Year - 1 : DateTime.Today.Year;

        private static readonly ILogger logger = LoggerFactory.GetLogger(LogPath, int.Parse(MinimumLoggingLevel));

        [TestMethod, TestCategory("Page Parsing")]
        public void TestClubParsing()
        {
            string url = "https://www.transfermarkt.pt/fc-barcelona/kader/verein/131/plus/1/galerie/0?saison_id=2011";
            ClubPage page = new ClubPage(new HAPConnection(), logger);
            page.Parse(url);

            var domain = page.Domain;
            Assert.IsNotNull(domain, "The returned Domain is null.");

            var mock = GetMockDomain();

            DomainElementsCheck(domain, mock.Children[0]);
            for (int i = 0; i < domain.Children.Count; i++)
            {
                DomainElementsCheck(domain.Children[i], mock.Children[0].Children[0]);
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

            var mock = GetMockDomain();

            DomainElementsCheck(domain, mock);
            
            // Clubs
            for (int i = 0; i < domain.Children.Count; i++)
            {
                var clubChild = domain.Children[i];
                DomainElementsCheck(clubChild, mock.Children[0]);

                for (int j = 0; j < clubChild.Children.Count; j++)
                {
                    var playerChild = clubChild.Children[j];
                    DomainElementsCheck(playerChild, mock.Children[0].Children[0]);
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

        private IDomain GetMockDomain()
        {
            var datePattern = @"^((0[1-9]|[12]\d|3[01])-(0[1-9]|1[0-2])-[12]\d{3} 00:00:00)$";
            var isoPattern = @"^[a-zA-Z]{3}$";
            var yearPattern = @"^[0-9]{4}$";
            var linkPattern = @"(^http.+)|(\/)";
            var captainPattern = @"(^[01]$)";
            var heightPattern = @"(^[12][0-9]{2}$)";
            var footPattern = @"^([R|L|A]{1})$";
            var positionPattern = @"^[a-zA-Z]{2}$";
            var mvPattern = @"[0-9]+";
            var shirtPattern = @"[0-9]+";
            var namePattern = @"^(\D)*$";
            var abbrevName = @"^(\D)*$";

            var competitionMock = new Competition();
            competitionMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Competition.Country() { Value = isoPattern });
            competitionMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Competition.Name() { Value = namePattern });
            competitionMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Competition.Season() { Value = yearPattern });
            competitionMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Competition.ImgUrl() { Value = linkPattern });
            competitionMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Competition.CountryImg() { Value = linkPattern });

            var clubMock = new Club();
            clubMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Club.Country() { Value = isoPattern });
            clubMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Club.Name() { Value = namePattern });
            clubMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Club.Season() { Value = yearPattern });
            clubMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Club.ImgUrl() { Value = linkPattern });
            clubMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Club.CountryImg() { Value = linkPattern });
            competitionMock.Children.Add(clubMock);

            var playerMock = new Player();
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.Name() { Value = namePattern });
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.ShortName() { Value = abbrevName });
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.BirthDate() { Value = datePattern });
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.Nationality() { Value = isoPattern });
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.Height() { Value = heightPattern });
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.PreferredFoot() { Value = footPattern });
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.Position() { Value = positionPattern });
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.ShirtNumber() { Value = shirtPattern });
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.Captain() { Value = captainPattern });
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.ClubArrivalDate() { Value = datePattern });
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.ContractExpirationDate() { Value = datePattern });
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.MarketValue() { Value = mvPattern });
            //playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.ImgUrl() { Value = linkPattern });
            playerMock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Player.ProfileUrl() { Value = linkPattern });
            clubMock.Children.Add(playerMock);

            return competitionMock;
        }

        private void DomainElementsCheck(IDomain domain, IDomain mock)
        {
            for (int i = 0; i < domain.Elements.Count; i++)
            {
                var e = domain.Elements[i];
                if (e.Value != null)
                {
                    Regex rgx = new Regex(mock.Elements[i].Value);
                    var str = string.Format("{0}", e.Value);
                    var m = rgx.Match(str);
                    Assert.IsTrue(rgx.IsMatch(str), $"{e.InternalName} returned an unexpected value: {e.Value}");
                }
            }
        }
    }
}
