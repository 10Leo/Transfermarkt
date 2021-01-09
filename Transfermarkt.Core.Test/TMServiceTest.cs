using LJMB.Common;
using LJMB.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Page.Scraper.Contracts;
using Page.Scraper.Exporter;
using Page.Scraper.Exporter.JSONExporter;
using System.Collections.Generic;
using System.Linq;
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
                BaseURL = BaseURL
            };
        }

        [TestMethod, TestCategory("TMService"), Priority(5)]
        [Ignore]
        public void TMServiceContinentParsingTest()
        {
            // Costly operation as it will parse every club in every competition in every continent
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

        [TestMethod, TestCategory("TMService"), Priority(3)]
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

        [TestMethod, TestCategory("TMService"), Priority(2)]
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

        [TestMethod, TestCategory("TMService"), Priority(3)]
        public void TMServiceMultipleIterationsTest()
        {
            IDomain domain = null;
            IDictionary<int, ContinentsPage> seasonContinents = TMService.SeasonContinents;
            List<(Option? option, int year, int? i1, int? i2, int? i3)> cmdHistory = new List<(Option? option, int year, int? i1, int? i2, int? i3)>();
            (Option? option, int year, int? i1, int? i2, int? i3) cmd = (null, 2020, null, null, null);


            // Peek one continent
            cmd = (Option.Peek, 2009, 1, null, null);
            domain = PassCommand(cmd);
            cmdHistory.Add(cmd);
            AssertTMService(cmd, cmdHistory);


            // Peek the same continent again
            cmd = (Option.Peek, 2009, 1, null, null);
            domain = PassCommand(cmd);
            cmdHistory.Add(cmd);
            AssertTMService(cmd, cmdHistory);

            // Peek the same continent again but in a different season
            cmd = (Option.Peek, 2015, 1, null, null);
            domain = PassCommand(cmd);
            cmdHistory.Add(cmd);
            AssertTMService(cmd, cmdHistory);

            // Peek the first competition on the same continent
            cmd = (Option.Peek, 2009, 1, 1, null);
            domain = PassCommand(cmd);
            cmdHistory.Add(cmd);
            AssertTMService(cmd, cmdHistory);


            // Peek another continent's competition's club
            cmd = (Option.Peek, 2009, 2, 1, 1);
            domain = PassCommand(cmd);
            cmdHistory.Add(cmd);
            AssertTMService(cmd, cmdHistory);


            // Peek all clubs in a competition
            for (int i = 0; i < 16; i++)
            {
                cmd = (Option.Peek, 2009, 1, 6, (i + 1));
                domain = PassCommand(cmd);
                cmdHistory.Add(cmd);
                AssertTMService(cmd, cmdHistory);
            }


            // Parse a competition
            cmd = (Option.Parse, 2009, 1, 2, null);
            domain = PassCommand(cmd);
            cmdHistory.Add(cmd);
            AssertTMService(cmd, cmdHistory);
        }

        private IDomain PassCommand((Option? option, int year, int? i1, int? i2, int? i3) cmd) {
            return TMService.Parse(cmd.year, cmd.i1, cmd.i2, cmd.i3, peek: (cmd.option == Option.Peek ? true : false));
        }

        private void AssertTMService((Option? option, int year, int? i1, int? i2, int? i3) cmd, List<(Option? option, int year, int? i1, int? i2, int? i3)> cmdHistory)
        {
            // validate continent and season
            string key = string.Format(TMService.KEY_PATTERN, cmd.year, cmd.i1);
            Assert.IsTrue(TMService.SeasonContinents.ContainsKey(cmd.year), $"Continent's season {cmd.year} not found.");
            ContinentsPage choice = TMService.SeasonContinents[cmd.year];
            Assert.IsTrue(choice != null, "A page of type ContinentPage should have been created by the Parse command.");

            var p = choice;

            YearTester(p.Year.Value, cmd.year);

            var cdt = cmdHistory.Where(c => c.option == Option.Parse && c.year == cmd.year && c.i1 == cmd.i1 && c.i2 == null);
            TestingConfigs.AssertParseLevel(cmd.option == Option.Peek, p.ParseLevel, p.Sections, cdt.Any());

            foreach (var childSection in p.Sections.ToList().OfType<ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>>())
            {
                Assert.IsTrue(childSection.Children != null && childSection.Children.Count > 0, $"Children Competitions should exist.");

                for (int i2 = 0; i2 < childSection.Children.Count; i2++)
                {
                    var competitionLink = childSection.Children[i2];
                    var index2 = i2 + 1;

                    // p -y year -i cmdi1 cmdi1.index2.*
                    var findParsed2 = cmdHistory.Where(
                        c => c.option == Option.Parse && c.year == cmd.year
                        && ((c.i1 == cmd.i1 && c.i2 == null) || (c.i1 == cmd.i1 && c.i2 == index2/* && c.i3 == null*/))
                    );
                    // f -y year -i cmdi1.index2
                    var findPeeked2 = cmdHistory.Where(
                        c => c.option == Option.Peek && c.year == cmd.year
                        && c.i1 == cmd.i1 && c.i2 == index2
                    );
                    if (findParsed2.Any() || findPeeked2.Any())
                    {
                        Assert.IsNotNull(competitionLink.Page, $"Page should not be null.");

                        //YearTester(competitionPage.Year.Value, cmd.year);

                        bool isCompetitionAlreadyParsed = false;
                        bool areAllCompetitionsChildrenClubsPeekedOrParsed = true;

                        foreach (var competitionsChildSection in competitionLink.Page.Sections.ToList().OfType<ChildsSection<HtmlAgilityPack.HtmlNode, ClubPage>>())
                        {
                            Assert.IsTrue(competitionsChildSection.Children != null && competitionsChildSection.Children.Count > 0, $"Children Competitions should exist.");

                            for (int i = 0; i < competitionsChildSection.Children.Count; i++)
                            {
                                areAllCompetitionsChildrenClubsPeekedOrParsed = cmdHistory.Any(
                                    fp => (fp.option == Option.Peek || fp.option == Option.Parse) && fp.year == cmd.year && fp.i1 == cmd.i1
                                    && fp.i2 == index2
                                    && fp.i3 == (i + 1)
                                );
                                if (areAllCompetitionsChildrenClubsPeekedOrParsed == false)
                                {
                                    break;
                                }
                            }

                            for (int i3 = 0; i3 < competitionsChildSection.Children.Count; i3++)
                            {
                                var index3 = i3 + 1;
                                var findParsed3 = cmdHistory.Where(c => c.option == Option.Parse && c.year == cmd.year && ((c.i1 == cmd.i1 && c.i2 == null) || (c.i1 == cmd.i1 && c.i2 == index2 && c.i3 == null) || (c.i1 == cmd.i1 && c.i2 == index2 && c.i3 == index3)));
                                var findPeeked3 = cmdHistory.Where(c => c.option == Option.Peek && c.year == cmd.year && c.i1 == cmd.i1 && c.i2 == index2 && c.i3 == index3);
                                
                                if (findParsed3.Any() || findPeeked3.Any())
                                {
                                    Assert.IsNotNull(competitionsChildSection.Children[i3].Page, $"Page should not be null.");
                                }
                                else
                                {
                                    Assert.IsNull(competitionsChildSection.Children[i3].Page, $"Page should be null.");
                                }
                            }
                        }

                        isCompetitionAlreadyParsed = findParsed2.Any(fp => (fp.i1 == cmd.i1 && fp.i2 == index2 && fp.i3 == null) || (fp.i1 == cmd.i1 && fp.i2 == null && fp.i3 == null));
                        TestingConfigs.AssertParseLevel(cmd.option == Option.Peek, competitionLink.Page.ParseLevel, competitionLink.Page.Sections, isCompetitionAlreadyParsed || areAllCompetitionsChildrenClubsPeekedOrParsed);
                    }
                    else
                    {
                        Assert.IsNull(competitionLink.Page, $"Page should be null.");
                    }
                }
            }
        }

        /// <summary>
        /// Checks if parse levels are OK within a Page
        /// </summary>
        private void ParseLevelTester()
        {
        }

        /// <summary>
        /// Checks if all children are OK within a ChildSection
        /// </summary>
        private void ChildrenTester()
        {
        }

        /// <summary>
        /// Tests if the selected year is OK in a  Page
        /// </summary>
        private void YearTester(int year, int y)
        {
            Assert.IsTrue(year == y, $"The page shoud have been set to get the {y} year.");
        }
    }
}
