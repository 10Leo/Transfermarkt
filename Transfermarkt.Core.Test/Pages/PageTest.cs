using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Pages;
using Transfermarkt.Logging;
using System.Linq;
using System.Collections.Generic;

namespace Transfermarkt.Core.Test.ParseHandling.Pages
{
    [TestClass]
    public class PageTest
    {
        private static int MinimumLoggingLevel { get; } = ConfigManager.GetAppSetting<int>(Keys.Config.MinimumLoggingLevel);
        private static string LogPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.LogPath);

        private static readonly ILogger logger = LoggerFactory.GetLogger(LogPath, MinimumLoggingLevel);

        [TestMethod, TestCategory("Page")]
        public void TestPageMethods()
        {
            ClubPage page = new ClubPage(new HAPConnection(), logger, null);
            var section = page["Club - Players Section"];

            Assert.IsNotNull(section, "The returned Section is null.");
            Assert.IsTrue(section.Name == "Club - Players Section", "The returned Section was different than the one expected.");
        }

        [TestMethod, TestCategory("Page")]
        public void TestPartialParsing()
        {
            //string url = "https://www.transfermarkt.pt/fc-barcelona/kader/verein/131/plus/1/galerie/0?saison_id=2011";
            //ClubPage page = new ClubPage(new HAPConnection(), logger, null);
            //var section = page["Club - Players Section"];
            //page.Parse(url, "Club - Players Section", new Link[] { new Link { Title = "" } });
            ////page.Parse(url, new Link[] { new Link { ID = "1.1.1", Title = "" } });

            //Assert.IsNotNull(section, "The returned Section is null.");
            //Assert.IsTrue(section.Name == "Club - Players Section", "The returned Section was different than the one expected.");


            string[] urls = new string[]
            {
                "https://www.transfermarkt.pt/wettbewerbe/europa",
                "https://www.transfermarkt.pt/wettbewerbe/amerika",
                "https://www.transfermarkt.pt/wettbewerbe/asien",
                "https://www.transfermarkt.pt/wettbewerbe/afrika"
            };
            var append = "/wettbewerbe?plus=1";

            ContinentPage continentPage = new ContinentPage(new HAPConnection(), logger, null);

            //TODO: consider passing url in constructor, making it this way always available to the functions, as it will be required by the constructor.
            continentPage.Connect(urls[0]);

            var sectionsToParse = new List<ISection>
            {
                continentPage["Continent Details"]
            };

            continentPage.Parse(sectionsToParse);

            //sectionsToParse = new List<ISection>
            //{
            //    continentPage["Continent - Competitions Section"]
            //};
            //continentPage.Parse(sectionsToParse, false);


            var childSection = (ChildsSection<HtmlAgilityPack.HtmlNode, IValue>)continentPage["Continent - Competitions Section"];
            childSection.Parse(false);


            var childrenToParse = childSection.Children.Where(u => u.Title == "Portugal-Liga NOS"
                                                                || u.Title == "Inglaterra-Premier League"
                                                                || u.Title == "Espanha-LaLiga"
                                                                || u.Title == "Itália-Serie A"
            );
            childSection.Parse(childrenToParse);

            var compPagePT = childSection["Portugal-Liga NOS"];
            compPagePT.Parse(parseChildren: true);

            //var continentSamePageSection = (IElementsSection<IElement<IValue>, IValue, HtmlAgilityPack.HtmlNode>)continentPage["Continent Details"];
            //continentSamePageSection.Parse();

            //IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlAgilityPack.HtmlNode> childSection = (IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlAgilityPack.HtmlNode>)continentPage["Continent - Competitions Section"];
            //var sectionUrls = childSection.Fetch();

            //childSection.Parse(sectionUrls.Where(u => u.Title == "Portugal-Liga NOS"));


            //IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlAgilityPack.HtmlNode> competition = childSection["SL Benfica"];

            //childSection["SL Benfica"].Parse();







            //string url = "https://www.transfermarkt.pt/serie-a/startseite/wettbewerb/IT1";
            //url = "https://www.transfermarkt.pt/liga-nos/startseite/wettbewerb/PO1/plus/?saison_id=2009";
            //CompetitionPage competitionPage = new CompetitionPage(new HAPConnection(), logger, "2013");
            //competitionPage.Connect(url);

            //IEnumerable<string> names = competitionPage.Sections.Select(s => s.Name);
            //var ss = competitionPage.Sections.Select(s => new { s.Name, T = s.GetType() });

            //var samePageSection = (IElementsSection<IElement<IValue>, IValue, HtmlAgilityPack.HtmlNode>)competitionPage["Competition Details"];
            //samePageSection.Parse();

            //childSection = (IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlAgilityPack.HtmlNode>)competitionPage["Competition - Clubs Section"];
            //sectionUrls = childSection.Fetch();

            //childSection.Parse(sectionUrls.Where(s => s.Title == "SL Benfica" || s.Title == "FC Porto"));
            //childSection.Parse(sectionUrls.Where(s => s.Title == "SL Benfica" || s.Title == "Sporting CP"));

            //var domain = competitionPage.Domain;
            //Assert.IsNotNull(domain, "The returned Domain is null.");
            //Assert.IsTrue(domain.Children.Count == (4 - 1), "There should be only 3 records as one of the 4 supplied links was a repetition and no repetition should be parsed.");
        }

        [TestMethod, TestCategory("Page Parsing")]
        public void TestClubParsing()
        {
            string url = "https://www.transfermarkt.pt/fc-barcelona/kader/verein/131/plus/1/galerie/0?saison_id=2011";

            ClubPage page = new ClubPage(new HAPConnection(), logger, null);
            //page.Parse(url);

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
            CompetitionPage page = new CompetitionPage(new HAPConnection(), logger, null);
            //page.Parse(url);

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
            string[] urls = new string[]
            {
                "https://www.transfermarkt.pt/wettbewerbe/europa",
                "https://www.transfermarkt.pt/wettbewerbe/amerika",
                "https://www.transfermarkt.pt/wettbewerbe/asien",
                "https://www.transfermarkt.pt/wettbewerbe/afrika"
            };
            var append = "/wettbewerbe?plus=1";

            ContinentPage page = null;
            foreach (var url in urls)
            {
                page = new ContinentPage(new HAPConnection(), logger, null);
                //page.Parse(url);
            }
        }
    }
}
