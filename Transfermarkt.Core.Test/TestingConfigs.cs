using Microsoft.VisualStudio.TestTools.UnitTesting;
using Page.Scraper.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.Test
{
    public struct Patterns
    {
        public const string date = @"^((0[1-9]|[12]\d|3[01])-(0[1-9]|1[0-2])-[12]\d{3})$";
        public const string iso = @"^[a-zA-Z]{3}$";
        public const string year = @"^[0-9]{4}$";
        public const string link = @"(^http.+)|(\/)";
        public const string captain = @"(^[01]$)";
        public const string height = @"(^[12][0-9]{2}$)";
        public const string foot = @"^([R|L|B]{1})$";
        public const string position = @"^[a-zA-Z]{2}$";
        public const string mv = @"[0-9]+";
        public const string shirt = @"[0-9]+";
        public const string name = ".+";//@"^(\D)*$";
        public const string abbrevName = @"^(\D)*$";
    }

    class TestingConfigs
    {
        public static readonly IDictionary<Type, string> PatternsMap = new Dictionary<Type, string> {
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

        public static readonly int currentSeason = (DateTime.Today.Year < 8) ? DateTime.Today.Year - 1 : DateTime.Today.Year;

        public static readonly IDictionary<string, (int id, string internalName)> clubs = new Dictionary<string, (int, string)>
        {
            ["Barcelona"] = (131, "fc-barcelona"),
            ["Nacional"] = (982, "cd-nacional"),
            ["V. Guimarães"] = (2420, "vitoria-sc"),
        };

        public static IDictionary<Nationality, (string internalName, string d1, string d2)> competitions = new Dictionary<Nationality, (string internalName, string d1, string d2)>
        {
            [Nationality.ITA] = ("serie-a", "IT1", "")
        };

        public static void DomainElementsCheck(IDomain domain)
        {
            string dateFormat = "dd-MM-yyyy";

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
                    value = ((DatetimeValue)e.Value).Value?.ToString(dateFormat, CultureInfo.InvariantCulture);
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
                    var pattern = TestingConfigs.PatternsMap[e.GetType()];
                    Regex rgx = new Regex(pattern);
                    var str = string.Format("{0}", value);
                    var m = rgx.Match(str);
                    Assert.IsTrue(rgx.IsMatch(str), $"{e.InternalName} returned an unexpected value: {value}");
                }
            }
        }

        public static void AssertParseLevel(bool peek, ParseLevel? parseLevel, IReadOnlyList<ISection> sections)
        {
            foreach (var section in sections)
            {
                switch (section.ChildrenType)
                {
                    case Children.NO:
                        Assert.IsTrue(section.ParseLevel == ParseLevel.Parsed);
                        break;
                    case Children.SAME_PAGE:
                        Assert.IsTrue(section.ParseLevel == ParseLevel.Parsed);
                        break;
                    case Children.DIFF_PAGE:
                        if (peek)
                        {
                            Assert.IsTrue(section.ParseLevel == ParseLevel.Peeked);
                        }
                        else
                        {
                            Assert.IsTrue(section.ParseLevel == ParseLevel.Parsed);
                        }
                        break;
                    default:
                        Assert.Fail("A New State was added and it's not yet contemplated in this test. Please add it to the switch condition.");
                        break;
                }
            }

            if (peek)
            {
                Assert.IsTrue(parseLevel == ParseLevel.Peeked, $"Page was expected to not have all sections parsed and as such its state must be the minimum child section level {ParseLevel.Peeked.ToString()}.");
            }
            else
            {
                Assert.IsTrue(parseLevel == ParseLevel.Parsed, $"Page was expected to have all sections parsed and as such its state must be {ParseLevel.Parsed.ToString()}.");
            }
        }
    }
}
