using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Pages;

namespace Transfermarkt.Core.Test.ParseHandling.Pages
{
    [TestClass]
    public class PageTest
    {
        private static IConfigurationManager config = new ConfigManager();

        private static string BaseURL { get; } = config.GetAppSetting("BaseURL");
        private static string PlusClubUrlFormat { get; } = config.GetAppSetting("PlusClubUrlFormatV2");
        private static string CompetitionUrlFormat { get; } = config.GetAppSetting("CompetitionUrlFormat");

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

        [TestMethod, TestCategory("Page Parsing")]
        public void TestClubParsing()
        {
            string url = "https://www.transfermarkt.pt/fc-barcelona/kader/verein/131/plus/1/galerie/0?saison_id=2011";
            ClubPage page = new ClubPage(new HAPConnection());
            page.Parse(url);
        }

        [TestMethod, TestCategory("Page Parsing")]
        public void TestCompetitionParsing()
        {
            string url = "https://www.transfermarkt.pt/serie-a/startseite/wettbewerb/IT1";
            url = "https://www.transfermarkt.pt/liga-nos/startseite/wettbewerb/PO1/plus/?saison_id=2019";
            CompetitionPage page = new CompetitionPage(new HAPConnection());
            page.Parse(url);

            var domain = page.Domain;
            Assert.IsNotNull(domain, "The returned Domain is null.");

            var mock = new Competition();
            mock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Competition.Country() { Value = "[a-zA-Z]{3}" });
            mock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Competition.Name() { Value = "[a-zA-Z0-9 ]+" });
            mock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Competition.Season() { Value = "[0-9]{4}" });
            mock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Competition.ImgUrl() { Value = "[a-zA-Z0-9 ]+" });
            mock.SetElement(new Transfermarkt.Core.ParseHandling.Elements.Competition.CountryImg() { Value = "[a-zA-Z0-9 ]+" });

            for (int i = 0; i < domain.Elements.Count; i++)
            {
                Regex rgx = new Regex(mock.Elements[i].Value);
                var str = string.Format("{0}", domain.Elements[i].Value);

                var m = rgx.Match(str);
                //Assert.IsTrue(rgx.Match(str), $"{domain.Elements[i].Value} is not a match.");
            }

            //foreach (var children in domain.Children)
            //{
            //    for (int i = 0; i < domain.Elements.Count; i++)
            //    {
            //        Regex rgx = new Regex(mock.Elements[i].Value);
            //        Assert.IsTrue(domain.Elements[i].Value == mock.Elements[i], "");
            //    }
            //}
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
    }
}
