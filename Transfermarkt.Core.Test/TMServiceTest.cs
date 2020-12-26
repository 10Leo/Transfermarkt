using LJMB.Common;
using LJMB.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Page.Scraper.Contracts;
using Page.Scraper.Exporter;
using Page.Scraper.Exporter.JSONExporter;
using System.Collections.Generic;
using Transfermarkt.Core.ParseHandling.Pages;
using Transfermarkt.Core.Service;

namespace Transfermarkt.Core.Test
{
    [TestClass]
    public class TMServiceTest
    {
        protected static string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);
        protected static string ContinentFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ContinentFileNameFormat);
        protected static string CompetitionFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.CompetitionFileNameFormat);
        protected static string ClubFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ClubFileNameFormat);
        protected static int MinimumLoggingLevel { get; } = ConfigManager.GetAppSetting<int>(Keys.Config.MinimumLoggingLevel);
        protected static string LogPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.LogPath);
        protected static string OutputFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.OutputFolderPath);
        protected static string Level1FolderFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.Level1FolderFormat);

        protected static IExporter Exporter { get; private set; }
        protected TMService TMService { get; private set; }

        [TestInitialize]
        public void Initialize()
        {
            Exporter = new JsonExporter(OutputFolderPath, Level1FolderFormat);

            TMService = new TMService
            {
                Logger = LoggerFactory.GetLogger((LogLevel)MinimumLoggingLevel),
                BaseURL = BaseURL,
                ContinentFileNameFormat = ContinentFileNameFormat,
                CompetitionFileNameFormat = CompetitionFileNameFormat,
                ClubFileNameFormat = ClubFileNameFormat
            };
        }

        [TestMethod]
        public void TMServiceContinentParsingTest()
        {
            IDomain domain = TMService.Parse(2010, 1);

            Assert.IsNotNull(domain, "The returned Domain is null.");

            TestingConfigs.DomainElementsCheck(domain);
            for (int i = 0; i < domain.Children.Count; i++)
            {
                var competitionChild = domain.Children[i];
                TestingConfigs.DomainElementsCheck(competitionChild);

                for (int j = 0; j < competitionChild.Children.Count; j++)
                {
                    var clubChild = competitionChild.Children[j];
                    TestingConfigs.DomainElementsCheck(clubChild);

                    for (int k = 0; k < clubChild.Children.Count; k++)
                    {
                        var playerChild = clubChild.Children[k];
                        TestingConfigs.DomainElementsCheck(playerChild);
                    }
                }
            }
        }

        [TestMethod]
        public void TMServiceCompetitionParsingTest()
        {
            IDomain domain = TMService.Parse(2010, 1, 1);

            Assert.IsNotNull(domain, "The returned Domain is null.");

            TestingConfigs.DomainElementsCheck(domain);
            for (int i = 0; i < domain.Children.Count; i++)
            {
                var clubChild = domain.Children[i];
                TestingConfigs.DomainElementsCheck(clubChild);

                for (int j = 0; j < clubChild.Children.Count; j++)
                {
                    var playerChild = clubChild.Children[j];
                    TestingConfigs.DomainElementsCheck(playerChild);
                }
            }
        }

        [TestMethod]
        public void TMServiceClubParsingTest()
        {
            IDomain domain = TMService.Parse(2010, 1, 1, 1);

            Assert.IsNotNull(domain, "The returned Domain is null.");

            TestingConfigs.DomainElementsCheck(domain);
            for (int i = 0; i < domain.Children.Count; i++)
            {
                TestingConfigs.DomainElementsCheck(domain.Children[i]);
            }
        }

        [TestMethod]
        public void TMServiceMultipleIterationsTest()
        {
            IDomain domain = null;
            int y = 2009;
            int continentIndex = 1;
            string key = string.Format(TMService.KEY_PATTERN, y, continentIndex);

            IDictionary<string, Link<HtmlAgilityPack.HtmlNode, ContinentPage>> seasonContinents = TMService.SeasonContinents;
            domain = TMService.Parse(y, continentIndex, peek: true);

            Link<HtmlAgilityPack.HtmlNode, ContinentPage> choice = seasonContinents[key];
            Assert.IsTrue(seasonContinents.ContainsKey(key), $"Continent's season {key} not found.");
            Assert.IsTrue(choice.Page != null, "A page of type ContinentPage should have been created by the Parse command.");
            Assert.IsTrue(choice.Page.Year == y, $"The page shoud have been set to get the {y} year.");

            TestingConfigs.AssertParseLevel(true, choice.Page.ParseLevel, choice.Page.Sections);

            foreach (var continentSection in choice.Page.Sections)
            {
                if (continentSection is ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>)
                {
                    var continentChildSec = continentSection as ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>;

                    Assert.IsTrue(continentChildSec.Children != null && continentChildSec.Children.Count > 0, $"Children Competitions should exist.");

                    foreach (var competitionChild in continentChildSec.Children)
                    {
                        Assert.IsNull(competitionChild.Page, $"Page should be null.");
                    }
                }
                else
                {
                    Assert.IsTrue(continentSection.ParseLevel == ParseLevel.Parsed, "These sections were already parsed.");
                }
            }
            //domain = TMService.Parse(y, continentIndex, 1, peek: true);
            //domain = TMService.Parse(y, continentIndex, 2, 1);
        }
    }
}
