using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.Exporter;
using Transfermarkt.Core.Pages;

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
            PlayerPage page = new PlayerPage("https://www.transfermarkt.pt/fc-barcelona/kader/verein/131/plus/1/galerie/0?saison_id=2011");

            page.Parse();
            page.Save();
        }
    }
}
