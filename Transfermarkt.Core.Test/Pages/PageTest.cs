using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.Exporter;
using Transfermarkt.Core.Pages;
using Transfermarkt.Core.Parsers.HtmlAgilityPack;
using Transfermarkt.Core.Parsers.HtmlAgilityPack.Player;

namespace Transfermarkt.Core.Test.Pages
{
    [TestClass]
    public class PageTest
    {
        private static string BaseURL { get; } = ConfigurationManager.AppSettings["BaseURL"].ToString();
        private static string PlusClubUrlFormat { get; } = ConfigurationManager.AppSettings["PlusClubUrlFormatV2"].ToString();
        private static string CompetitionUrlFormat { get; } = ConfigurationManager.AppSettings["CompetitionUrlFormat"].ToString();

        private static IParser conn;
        private static IExporter exporter;

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

        [TestMethod]
        public void TestClubParsing()
        {
            string url = "https://www.transfermarkt.pt/fc-barcelona/kader/verein/131/plus/1/galerie/0?saison_id=2011";
            IHAPClubPage page = new ClubPage(url);
            page.Parse();
            page.Save();
        }

        [TestMethod]
        public void TestCompetitionParsing()
        {
            string url = "https://www.transfermarkt.pt/serie-a/startseite/wettbewerb/IT1";
            CompetitionPage page = new CompetitionPage(url);
            page.Parse();
            page.Save();
        }

        [TestMethod]
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
                ContinentPage page = new ContinentPage(url);
                page.Parse();
                page.Save();
            }
        }
    }
}
