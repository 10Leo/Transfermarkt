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
                BaseURL = BaseURL,
                ContinentFileNameFormat = ContinentFileNameFormat,
                CompetitionFileNameFormat = CompetitionFileNameFormat,
                ClubFileNameFormat = ClubFileNameFormat
            };
        }

        [TestMethod, Priority(3)]
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
            int competitionIndex = 1;
            int clubIndex = 1;
            List<int> parsedI1 = new List<int>();
            List<int> parsedI2 = new List<int>();
            List<int> parsedI3 = new List<int>();
            string key = string.Format(TMService.KEY_PATTERN, y, continentIndex);
            IDictionary<string, Link<HtmlAgilityPack.HtmlNode, ContinentPage>> seasonContinents = TMService.SeasonContinents;


            var op = ParseLevel.Peeked;

            // Peek one continent
            domain = TMService.Parse(y, continentIndex, peek: (op == ParseLevel.Parsed ? false : true));

            Assert.IsTrue(seasonContinents.ContainsKey(key), $"Continent's season {key} not found.");
            Link<HtmlAgilityPack.HtmlNode, ContinentPage> choice = seasonContinents[key];

            Assert.IsTrue(choice.Page != null, "A page of type ContinentPage should have been created by the Parse command.");
            //YearTester(choice.Page.Year.Value, y);
            //PageTester(choice.Page);

            PageTester(choice.Page, (op == ParseLevel.Parsed ? true : false), y, parsedI1, parsedI2, parsedI3, competitionIndex);

            parsedI1.Add(continentIndex);






            op = ParseLevel.Peeked;
            domain = TMService.Parse(y, continentIndex, /*competitionIndex,*/ peek: true);


            var p = (ContinentPage)choice.Page;

            YearTester(p.Year.Value, y);
            TestingConfigs.AssertParseLevel(true, p.ParseLevel, p.Sections);

            foreach (var childSection in p.Sections.ToList().OfType<ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>>())
            {
                Assert.IsTrue(childSection.Children != null && childSection.Children.Count > 0, $"Children Competitions should exist.");

                for (int i2 = 0; i2 < childSection.Children.Count; i2++)
                {
                    var competitionPage = childSection.Children[i2].Page;
                    if (!parsedI2.Contains(i2 + 1))
                    {
                        Assert.IsNull(competitionPage, $"Page should be null.");
                    }
                    else
                    {
                        Assert.IsNotNull(competitionPage, $"Page should not be null.");


                        YearTester(competitionPage.Year.Value, y);
                        TestingConfigs.AssertParseLevel(true, competitionPage.ParseLevel, competitionPage.Sections);

                        foreach (var competitionsChildSection in competitionPage.Sections.ToList().OfType<ChildsSection<HtmlAgilityPack.HtmlNode, ClubPage>>())
                        {
                            Assert.IsTrue(competitionsChildSection.Children != null && competitionsChildSection.Children.Count > 0, $"Children Competitions should exist.");

                            for (int i3 = 0; i3 < competitionsChildSection.Children.Count; i3++)
                            {
                                if (!parsedI3.Contains(i3 + 1))
                                {
                                    Assert.IsNull(competitionsChildSection.Children[i3].Page, $"Page should be null.");
                                }
                                else
                                {
                                    Assert.IsNotNull(competitionsChildSection.Children[i3].Page, $"Page should not be null.");
                                }
                            }
                        }
                    }
                }
            }

            if (!parsedI1.Contains(continentIndex))
            {
                parsedI1.Add(continentIndex);
            }











            op = ParseLevel.Peeked;
            domain = TMService.Parse(y, continentIndex, competitionIndex, peek: true);

            if (!parsedI1.Contains(continentIndex))
            {
                parsedI1.Add(continentIndex);
            }
            if (!parsedI2.Contains(competitionIndex))
            {
                parsedI2.Add(competitionIndex);
            }



            p = (ContinentPage)choice.Page;

            YearTester(p.Year.Value, y);
            TestingConfigs.AssertParseLevel(true, p.ParseLevel, p.Sections);

            foreach (var childSection in p.Sections.ToList().OfType<ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>>())
            {
                Assert.IsTrue(childSection.Children != null && childSection.Children.Count > 0, $"Children Competitions should exist.");

                for (int i2 = 0; i2 < childSection.Children.Count; i2++)
                {
                    var competitionPage = childSection.Children[i2].Page;
                    if (!parsedI2.Contains(i2 + 1))
                    {
                        Assert.IsNull(competitionPage, $"Page should be null.");
                    }
                    else
                    {
                        Assert.IsNotNull(competitionPage, $"Page should not be null.");


                        //YearTester(competitionPage.Year.Value, y);
                        TestingConfigs.AssertParseLevel(true, competitionPage.ParseLevel, competitionPage.Sections);

                        foreach (var competitionsChildSection in competitionPage.Sections.ToList().OfType<ChildsSection<HtmlAgilityPack.HtmlNode, ClubPage>>())
                        {
                            Assert.IsTrue(competitionsChildSection.Children != null && competitionsChildSection.Children.Count > 0, $"Children Competitions should exist.");

                            for (int i3 = 0; i3 < competitionsChildSection.Children.Count; i3++)
                            {
                                if (!parsedI3.Contains(i3 + 1))
                                {
                                    Assert.IsNull(competitionsChildSection.Children[i3].Page, $"Page should be null.");
                                }
                                else
                                {
                                    Assert.IsNotNull(competitionsChildSection.Children[i3].Page, $"Page should not be null.");
                                }
                            }
                        }
                    }
                }
            }

            










            //domain = TMService.Parse(y, continentIndex, 2, 1);
        }

        private void PageTester(IPage<IDomain, HtmlAgilityPack.HtmlNode> page, bool parse, int y, List<int> parsedI1, List<int> parsedI2, List<int> parsedI3, int? i1 = null, int? i2 = null, int? i3 = null)
        {
            if (page is ContinentPage)
            {
                var p = (ContinentPage)page;

                YearTester(p.Year.Value, y);
                TestingConfigs.AssertParseLevel(!parse, p.ParseLevel, p.Sections);

                foreach (var section in page.Sections)
                {
                    if (section is ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>)
                    {
                        var childSection = section as ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>;

                        Assert.IsTrue(childSection.Children != null && childSection.Children.Count > 0, $"Children Competitions should exist.");

                        foreach (var childLink in childSection.Children)
                        {
                            if (parse)
                            {
                                //PageTester(competitionChild.Page);
                                PageTester(childLink.Page, parse, y);
                            }
                            else
                            {
                                if (!parsedI1.Contains(i1.Value))
                                {
                                    Assert.IsNull(childLink.Page, $"Page should be null.");
                                }
                            }
                        }
                    }
                    else
                    {
                        //Assert.IsTrue(continentSection.ParseLevel == ParseLevel.Parsed, "These sections were already parsed.");
                    }
                }
            }
            else if (page is CompetitionPage)
            {
                var p = (CompetitionPage)page;

                YearTester(p.Year.Value, y);
                TestingConfigs.AssertParseLevel(!parse, p.ParseLevel, p.Sections);

                foreach (var section in page.Sections)
                {
                    if (section is ChildsSection<HtmlAgilityPack.HtmlNode, ClubPage>)
                    {
                        var childSection = section as ChildsSection<HtmlAgilityPack.HtmlNode, ClubPage>;

                        Assert.IsTrue(childSection.Children != null && childSection.Children.Count > 0, $"Children Clubs should exist.");

                        foreach (var childLink in childSection.Children)
                        {
                            if (parse)
                            {
                                //PageTester(competitionChild.Page);
                                PageTester(childLink.Page, parse, y);
                            }
                            else
                            {
                                Assert.IsNull(childLink.Page, $"Page should be null.");
                            }
                        }
                    }
                    else
                    {
                        //Assert.IsTrue(continentSection.ParseLevel == ParseLevel.Parsed, "These sections were already parsed.");
                    }
                }
            }
            else if (page is ClubPage)
            {
                var p = (ClubPage)page;

                TestingConfigs.AssertParseLevel(!parse, p.ParseLevel, p.Sections);

                foreach (var section in page.Sections)
                {
                    
                }
            }
        }

        private void PageTester(IPage<IDomain, HtmlAgilityPack.HtmlNode> page, bool parse, int y)
        {
            if (page is ContinentPage)
            {
                var p = (ContinentPage)page;

                YearTester(p.Year.Value, y);
                TestingConfigs.AssertParseLevel(!parse, p.ParseLevel, p.Sections);

                foreach (var section in page.Sections)
                {
                    if (section is ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>)
                    {
                        var childSection = section as ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>;

                        Assert.IsTrue(childSection.Children != null && childSection.Children.Count > 0, $"Children Competitions should exist.");

                        foreach (var childLink in childSection.Children)
                        {
                            if (parse)
                            {
                                //PageTester(competitionChild.Page);
                                PageTester(childLink.Page, parse, y);
                            }
                            else
                            {
                                Assert.IsNull(childLink.Page, $"Page should be null.");
                            }
                        }
                    }
                    else
                    {
                        //Assert.IsTrue(continentSection.ParseLevel == ParseLevel.Parsed, "These sections were already parsed.");
                    }
                }
            }
            else if (page is CompetitionPage)
            {
                var p = (CompetitionPage)page;

                YearTester(p.Year.Value, y);
                TestingConfigs.AssertParseLevel(!parse, p.ParseLevel, p.Sections);

                foreach (var section in page.Sections)
                {
                    if (section is ChildsSection<HtmlAgilityPack.HtmlNode, ClubPage>)
                    {
                        var childSection = section as ChildsSection<HtmlAgilityPack.HtmlNode, ClubPage>;

                        Assert.IsTrue(childSection.Children != null && childSection.Children.Count > 0, $"Children Clubs should exist.");

                        foreach (var childLink in childSection.Children)
                        {
                            if (parse)
                            {
                                //PageTester(competitionChild.Page);
                                PageTester(childLink.Page, parse, y);
                            }
                            else
                            {
                                Assert.IsNull(childLink.Page, $"Page should be null.");
                            }
                        }
                    }
                    else
                    {
                        //Assert.IsTrue(continentSection.ParseLevel == ParseLevel.Parsed, "These sections were already parsed.");
                    }
                }
            }
            else if (page is ClubPage)
            {
                var p = (ClubPage)page;

                TestingConfigs.AssertParseLevel(!parse, p.ParseLevel, p.Sections);

                foreach (var section in page.Sections)
                {

                }
            }
        }

        private void PageTester(IPage<IDomain, HtmlAgilityPack.HtmlNode> page)
        {
            foreach (var continentSection in page.Sections)
            {
                if (continentSection is ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>)
                {
                    var continentChildSec = continentSection as ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>;

                    Assert.IsTrue(continentChildSec.Children != null && continentChildSec.Children.Count > 0, $"Children Competitions should exist.");

                    foreach (var competitionChild in continentChildSec.Children)
                    {
                        Assert.IsNull(competitionChild.Page, $"Page should be null.");
                        PageTester(competitionChild.Page);
                    }
                }
                else
                {
                    Assert.IsTrue(continentSection.ParseLevel == ParseLevel.Parsed, "These sections were already parsed.");
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
}
