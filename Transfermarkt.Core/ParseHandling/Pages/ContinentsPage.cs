using HtmlAgilityPack;
using LJMB.Common;
using LJMB.Logging;
using Page.Scraper.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core.ParseHandling.Pages
{
    public class ContinentsPage : Page<IValue, HtmlNode>
    {
        //TODO: logger should come from the top level layer and not instantiated in here
        public ILogger Logger { get; set; } = LoggerFactory.GetLogger(LogLevel.Milestone);
        public int? Year { get; set; }

        public ContinentsPage() : base(new HAPConnection())
        {
            //TODO: if this constructor is called, the Year prop will not be used, as at the moment of this call is yet to be set
            Init();
        }

        //TODO: section names should be hold as variables in the page and passed to its child sections
        public ContinentsPage(HAPConnection connection, ILogger logger, int? year) : base(connection)
        {
            this.Logger = logger;
            this.Year = year;

            Init();
        }

        private void Init()
        {
            this.Domain = new Continents();

            this.Sections = new List<ISection>
            {
                new ContinentsContinentsPageSection(this, Logger, Year)
            };

            this.OnBeforeParse += (o, e) =>
            {
                Logger.LogMessage(LogLevel.Milestone, new List<string> { $"EVT: Started parsing.", $"URL: {e.Url}" });
            };

            this.OnAfterParse += (o, e) =>
            {
                Logger.LogMessage(LogLevel.Milestone, new List<string> { $"EVT: Finished parsing.", $"URL: {e.Url}" });
            };
        }
    }

    public class ContinentsContinentsPageSection : ChildsSection<HtmlNode, ContinentPage>
    {
        public static readonly string SectionName = "Continents - Continents Section";
        public string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);
        public int? Season { get; }

        public ContinentsContinentsPageSection(IPage<IDomain, HtmlNode> page, ILogger logger, int? year) : base(SectionName, page, page.Connection)
        {
            this.Season = year;
            this.ChildPage = new ContinentPage();

            this.GetUrls = () =>
            {
                var continentCodeType = typeof(ContinentCode);

                IList<Link<HtmlNode, ContinentPage>> urls = new List<Link<HtmlNode, ContinentPage>> {
                    new Link<HtmlNode, ContinentPage>{ Title = "Europe", Url = string.Format("{0}", BaseURL + "/wettbewerbe/europa"), Identifiers = new Dictionary<string, string> { { continentCodeType.Name, Actors.ContinentCode.EU.ToString() } } },
                    new Link<HtmlNode, ContinentPage>{ Title = "America", Url = string.Format("{0}", BaseURL + "/wettbewerbe/amerika"), Identifiers = new Dictionary<string, string> { { continentCodeType.Name, Actors.ContinentCode.A.ToString() } } },
                    new Link<HtmlNode, ContinentPage>{ Title = "Asia", Url = string.Format("{0}", BaseURL + "/wettbewerbe/asien"), Identifiers = new Dictionary<string, string> { { continentCodeType.Name, Actors.ContinentCode.AS.ToString() } } },
                    new Link<HtmlNode, ContinentPage>{ Title = "Africa", Url = string.Format("{0}", BaseURL + "/wettbewerbe/afrika"), Identifiers = new Dictionary<string, string> { { continentCodeType.Name, Actors.ContinentCode.AF.ToString() } } }
                };

                return urls;
            };
        }
    }
}
