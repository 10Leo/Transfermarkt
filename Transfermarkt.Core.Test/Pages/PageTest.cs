using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Pages;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.Test.ParseHandling.Pages
{
    [TestClass]
    public class PageTest
    {
        private static int MinimumLoggingLevel { get; } = ConfigManager.GetAppSetting<int>(Keys.Config.MinimumLoggingLevel);
        private static string LogPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.LogPath);

        private static readonly ILogger logger = LoggerFactory.GetLogger(LogPath, MinimumLoggingLevel);

        [TestMethod, TestCategory("Page Parsing")]
        public void TestClubParsing()
        {
            string url = "https://www.transfermarkt.pt/fc-barcelona/kader/verein/131/plus/1/galerie/0?saison_id=2011";

            ClubPage page = new ClubPage(new HAPConnection(), logger, null);
            page.Parse(url);

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
            page.Parse(url);

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
                page.Parse(url);
            }
        }
    }
}
