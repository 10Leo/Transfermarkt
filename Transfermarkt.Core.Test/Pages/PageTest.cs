﻿using LJMB.Common;
using LJMB.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Page.Scraper.Contracts;
using System.Collections.Generic;
using System.Linq;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Elements.Continent;
using Transfermarkt.Core.ParseHandling.Pages;

namespace Transfermarkt.Core.Test.ParseHandling.Pages
{
    [TestClass]
    public class PageTest
    {
        private static int MinimumLoggingLevel { get; } = ConfigManager.GetAppSetting<int>(Keys.Config.MinimumLoggingLevel);
        private static string LogPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.LogPath);

        private static readonly ILogger logger = LoggerFactory.GetLogger((LogLevel)MinimumLoggingLevel);

        private readonly string ptComp = "Portugal-Liga NOS", engComp = "Inglaterra-Premier League", spaComp = "Espanha-LaLiga", itaComp = "Itália-Serie A";

        private readonly string append = "/wettbewerbe?plus=1";
        private readonly IDictionary<Actors.ContinentCode, string> urls = new Dictionary<Actors.ContinentCode, string>
        {
            { Actors.ContinentCode.EU, "https://www.transfermarkt.pt/wettbewerbe/europa" },
            { Actors.ContinentCode.A, "https://www.transfermarkt.pt/wettbewerbe/amerika" },
            { Actors.ContinentCode.AS, "https://www.transfermarkt.pt/wettbewerbe/asien" },
            { Actors.ContinentCode.AF, "https://www.transfermarkt.pt/wettbewerbe/afrika" }
        };

        [TestMethod, TestCategory("Page Parsing")]
        public void TestClubParsing()
        {
            string url = "https://www.transfermarkt.pt/fc-barcelona/kader/verein/131/plus/1/galerie/0?saison_id=2011";

            ClubPage page = new ClubPage();
            page.Connect(url);
            page.Parse(parseChildren: true);

            TestingConfigs.AssertParseLevel(false, page.ParseLevel, page.Sections, true);

            var domain = page.Domain;
            Assert.IsNotNull(domain, "The returned Domain is null.");

            TestingConfigs.DomainElementsCheck(domain);
            for (int i = 0; i < domain.Children.Count; i++)
            {
                TestingConfigs.DomainElementsCheck(domain.Children[i]);
            }
        }

        [TestMethod, TestCategory("Page Parsing")]
        public void TestCompetitionParsing()
        {
            string url = "https://www.transfermarkt.pt/serie-a/startseite/wettbewerb/IT1";
            url = "https://www.transfermarkt.pt/liga-nos/startseite/wettbewerb/PO1/plus/?saison_id=2009";
            CompetitionPage page = new CompetitionPage();
            page.Connect(url);
            page.Parse(parseChildren: true);

            TestingConfigs.AssertParseLevel(false, page.ParseLevel, page.Sections, true);

            var domain = page.Domain;
            Assert.IsNotNull(domain, "The returned Domain is null.");

            TestingConfigs.DomainElementsCheck(domain);

            // Clubs
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

        [TestMethod, TestCategory("Page Parsing")]
        public void TestContinentParsing()
        {
            //foreach (KeyValuePair<Actors.ContinentCode, string> url in urls)
            //{
            //    ContinentPage page = new ContinentPage(new HAPConnection(), logger, null);
            //    page.Connect(url.Value);
            //    page.Parse();
            //}

            ContinentPage continentPage = new ContinentPage(new HAPConnection(), logger, 2008);
            continentPage.Connect(urls[Actors.ContinentCode.EU]);

            var continentSectionsToParse = new List<ISection> { continentPage["Continent Details"] };
            continentPage.Parse(continentSectionsToParse);
            Assert.IsTrue(((ContinentCode)continentPage.Domain.Elements.FirstOrDefault(e => e.InternalName == "Code")).Value.Value == Actors.ContinentCode.EU);
            Assert.IsTrue(continentPage.Domain.Children.Count == 0, "No children should exist yet as no ChildSection was passed to be parsed.");

            foreach (var section in continentSectionsToParse)
            {
                Assert.IsTrue(section.ParseLevel == ParseLevel.Parsed, "These sections were already parsed.");
            }
            Assert.IsTrue(continentPage.ParseLevel == ParseLevel.NotYet, "Page wasn't parsed yet, only one of its section.");

            var continentChildSection = (ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>)continentPage["Continent - Competitions Section"];
            Assert.IsNotNull(continentChildSection, "The returned Section is null.");
            Assert.IsTrue(continentChildSection.Name == "Continent - Competitions Section", "The returned Section was different than the one expected.");

            continentChildSection.Parse(false);
            Assert.IsTrue(continentChildSection.Children.Count > 0, "Children Links should have been fetched.");
            Assert.IsTrue(continentPage.Domain.Children.Count == 0, "No domain children should exist yet as the param parseChildren was set to false.");
            Assert.IsTrue(continentChildSection.ParseLevel == ParseLevel.Peeked, $"These section state should be {ParseLevel.Peeked.ToString()}.");

            IList<string> competitionLinksToParse = new List<string> { spaComp };

            var continentChildrenCompetitionsToParse = continentChildSection.Children.Where(u => competitionLinksToParse.Contains(u.Title));
            continentChildSection.Parse(continentChildrenCompetitionsToParse, true);
            Assert.IsTrue(continentPage.Domain.Children.Count == competitionLinksToParse.Count(), $"There should exist {competitionLinksToParse.Count} children.");

            foreach (var continentChildCompetition in continentChildrenCompetitionsToParse)
            {
                TestingConfigs.AssertParseLevel(false, continentChildCompetition.Page.ParseLevel, continentChildCompetition.Page.Sections, true);

                foreach (var competitionSection in continentChildCompetition.Page.Sections)
                {
                    if (competitionSection is ChildsSection<HtmlAgilityPack.HtmlNode, ClubPage>)
                    {
                        var childSec = competitionSection as ChildsSection<HtmlAgilityPack.HtmlNode, ClubPage>;

                        foreach (var clubChild in childSec.Children)
                        {
                            TestingConfigs.AssertParseLevel(false, clubChild.Page.ParseLevel, clubChild.Page.Sections, true);
                        }
                    }
                    else
                    {
                        Assert.IsTrue(competitionSection.ParseLevel == ParseLevel.Parsed, "These sections were already parsed.");
                    }
                }
            }

            var ctp = competitionLinksToParse.Select(l => l.Split('-')?[1]);
            for (int i = 0; i < continentPage.Domain.Children.Count; i++)
            {
                var childCompetition = continentPage.Domain.Children[i];

                Assert.IsTrue(ctp.Contains(((Core.ParseHandling.Elements.Competition.Name)childCompetition.Elements.FirstOrDefault(e => e.InternalName == "Name")).Value.Value), "Parsed a different competition children than the one expected.");
            }


            var domain = continentPage.Domain;
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

        [TestMethod, TestCategory("Page Parsing")]
        public void TestContinentsParsing()
        {
            string url = "https://www.transfermarkt.pt";

            ContinentsPage continentsPage = new ContinentsPage(new HAPConnection(), logger, 2010);
            continentsPage.Connect(url);
            continentsPage.Parse(parseChildren: false);

            TestingConfigs.AssertParseLevel(false, continentsPage.ParseLevel, continentsPage.Sections, false);

            var domain = continentsPage.Domain;
            Assert.IsNotNull(domain, "The returned Domain is null.");


            var continentsChildSection = (ChildsSection<HtmlAgilityPack.HtmlNode, ContinentPage>)continentsPage[ContinentsCompetitionsPageSection.SectionName];

            Assert.IsTrue(continentsChildSection.Children != null && continentsChildSection.Children.Count > 0, "Children should exist.");

            Link<HtmlAgilityPack.HtmlNode, ContinentPage> continentsChildrenContinentToParse = continentsChildSection.Children.FirstOrDefault(c => c.Title == "Europe");
            continentsChildSection.Parse(new List<Link<HtmlAgilityPack.HtmlNode, ContinentPage>> { continentsChildrenContinentToParse }, false);

            Assert.IsNotNull(continentsChildrenContinentToParse.Page, $"Page should not be null.");


            //TestingConfigs.DomainElementsCheck(domain);
            //for (int i = 0; i < domain.Children.Count; i++)
            //{
            //    TestingConfigs.DomainElementsCheck(domain.Children[i]);
            //}
        }

        [TestMethod, TestCategory("Page Parsing")]
        public void TestPartialParsing()
        {
            ContinentPage continentPage = new ContinentPage(new HAPConnection(), logger, 2009);

            //TODO: consider passing url in constructor making it a required param and as a result, always available to the functions.
            continentPage.Connect(urls[Actors.ContinentCode.EU]);

            var sectionsToParse = new List<ISection> { continentPage["Continent Details"] };
            continentPage.Parse(sectionsToParse);

            Assert.IsTrue(((ContinentCode)continentPage.Domain.Elements.FirstOrDefault(e => e.InternalName == "Code")).Value.Value == Actors.ContinentCode.EU);
            Assert.IsTrue(continentPage.Domain.Children.Count == 0, "No children should exist yet as no ChildSection was passed to be parsed.");

            //sectionsToParse = new List<ISection> { continentPage["Continent - Competitions Section"] };
            //continentPage.Parse(sectionsToParse, false);

            var childSection = (ChildsSection<HtmlAgilityPack.HtmlNode, CompetitionPage>)continentPage["Continent - Competitions Section"];
            Assert.IsNotNull(childSection, "The returned Section is null.");
            Assert.IsTrue(childSection.Name == "Continent - Competitions Section", "The returned Section was different than the one expected.");

            childSection.Parse(false);
            Assert.IsTrue(childSection.Children.Count > 0, "Children Links should have been fetched.");
            Assert.IsTrue(continentPage.Domain.Children.Count == 0, "No domain children should exist yet as the param parseChildren was set to false.");

            IList<string> linksToParse = new List<string> { ptComp, engComp, spaComp, itaComp };
            IEnumerable<string> nonExistingLinksToParse = new List<string> { "Non-Existant League 01", "Non-Existant League 02" };

            var childrenToParse = childSection.Children.Where(u => linksToParse.Contains(u.Title) || nonExistingLinksToParse.Contains(u.Title));
            childSection.Parse(childrenToParse);
            Assert.IsTrue(continentPage.Domain.Children.Count == linksToParse.Count(), $"There should exist {linksToParse.Count} children.");

            var ctp = linksToParse.Select(l => l.Split('-')?[1]);
            for (int i = 0; i < continentPage.Domain.Children.Count; i++)
            {
                var childCompetition = continentPage.Domain.Children[i];

                Assert.IsTrue(ctp.Contains(((Core.ParseHandling.Elements.Competition.Name)childCompetition.Elements.FirstOrDefault(e => e.InternalName == "Name")).Value.Value), "Parsed a different competition children than the one expected.");
            }


            foreach (string pageName in linksToParse)
            {
                IPage<IDomain, HtmlAgilityPack.HtmlNode> compPage = childSection[new Dictionary<string, string> { { "Title", pageName } }];
                Assert.IsNotNull(compPage, $"The returned Page {pageName} is null.");
                Assert.IsTrue(compPage.Domain.Children.Count == 0, $"No Club children should exist for {pageName}.");
            }


            IPage<IDomain, HtmlAgilityPack.HtmlNode> compPagePT = childSection[new Dictionary<string, string> { { "Nationality", Actors.Nationality.PRT.ToString() } }];
            Assert.IsNotNull(compPagePT, $"The returned Page {ptComp} is null.");

            compPagePT.Parse(parseChildren: true);
            Assert.IsTrue(compPagePT.Domain.Children.Count > 0, $"There should exist club children on {ptComp}.");
            foreach (string pageName in linksToParse.Where(l => l != ptComp))
            {
                IPage<IDomain, HtmlAgilityPack.HtmlNode> compPage = childSection[new Dictionary<string, string> { { "Title", pageName } }];
                Assert.IsNotNull(compPage, $"The returned Page {pageName} is null.");
                Assert.IsTrue(compPage.Domain.Children.Count == 0, $"No Club children should exist for {pageName}.");
            }


            var domain = continentPage.Domain;
            Assert.IsNotNull(domain, "The returned Domain is null.");
        }
    }
}
