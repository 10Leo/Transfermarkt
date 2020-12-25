using HtmlAgilityPack;
using LJMB.Logging;
using Page.Scraper.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Pages;

namespace Transfermarkt.Core.Service
{
    public class TMService
    {
        private const string Message = "Continent not found";
        private static readonly string KEY_ERROR = "Specified key doesn't exist.";
        public static readonly string KEY_PATTERN = "{0}.{1}";
        
        public string BaseURL { get; set; }
        public string ContinentFileNameFormat { get; set; }
        public string CompetitionFileNameFormat { get; set; }
        public string ClubFileNameFormat { get; set; }

        public ILogger Logger { get; set; }

        //TODO: create Page to represent these links
        public IDictionary<string, Link<HtmlNode, CompetitionPage>> Continents { get; private set; }

        public readonly IDictionary<string, (Link<HtmlNode, CompetitionPage> L, ContinentPage P)> SeasonContinents = null;

        public (Link<HtmlNode, CompetitionPage> L, ContinentPage P) Choice { get; }

        public TMService()
        {
            Continents = new Dictionary<string, Link<HtmlNode, CompetitionPage>>
            {
                ["1"] = (new Link<HtmlNode, CompetitionPage> { Title = "Europe", Url = $"/wettbewerbe/europa" }),
                ["2"] = (new Link<HtmlNode, CompetitionPage> { Title = "America", Url = $"/wettbewerbe/amerika" }),
                ["3"] = (new Link<HtmlNode, CompetitionPage> { Title = "Asia", Url = $"/wettbewerbe/asien" }),
                ["4"] = (new Link<HtmlNode, CompetitionPage> { Title = "Africa", Url = $"/wettbewerbe/afrika" })
            };
            SeasonContinents = new Dictionary<string, (Link<HtmlNode, CompetitionPage>, ContinentPage)>();
        }

        public IDomain Parse(int year, int? continentsIndex = null, int? competitionsIndex = null, int? clubsIndex = null, bool peek = false)
        {
            if (!continentsIndex.HasValue)
            {
                // Parse all continents - must be careful as it's a very expensive action.
                return null;
            }

            bool allPassedIndexesAreValid =
                (continentsIndex.HasValue && continentsIndex.Value > 0)
                || (competitionsIndex.HasValue && competitionsIndex.Value > 0)
                || (clubsIndex.HasValue && clubsIndex.Value > 0);
            if (!allPassedIndexesAreValid)
            {
                throw new IndexOutOfRangeException("Passed indexes must be all positive numbers");
            }

            var key = GenerateKey(year, continentsIndex.Value);

            if (!ContinentExists(continentsIndex.Value))
            {
                throw new KeyNotFoundException(Message);
            }
            AddNewYearContinentIfDoesntExist(year, continentsIndex.Value);

            (Link<HtmlNode, CompetitionPage> Link, ContinentPage Page) choice = GetSeasonContinent(key);
            if (choice.Link == null)
            {
                return null;
            }
            if (choice.Page == null)
            {
                choice.Page = new ContinentPage(new HAPConnection(), Logger, year);
                SeasonContinents[key] = choice;
            }

            if (!choice.Page.Connection.IsConnected)
            {
                choice.Page.Connect($"{BaseURL}{choice.Link.Url}");
            }

            //TODO: cretae enum to hold parse and peek values and pass them to the Parse methods
            bool shouldParse = (!peek && !competitionsIndex.HasValue);
            choice.Page.Parse(parseChildren: shouldParse);

            if (!competitionsIndex.HasValue)
            {
                return choice.Page.Domain;
            }

            var continentCompetitionsSection = (ChildsSection<HtmlNode, CompetitionPage>)choice.Page["Continent - Competitions Section"];
            if (continentCompetitionsSection == null)
            {
                throw new Exception("Parser was not able to continue as the section was not found.");
            }

            Link<HtmlNode, CompetitionPage> chosenCompetitionLink = continentCompetitionsSection.Children[competitionsIndex.Value - 1];
            shouldParse = (!peek && !clubsIndex.HasValue);
            continentCompetitionsSection.Parse(new[] { chosenCompetitionLink }, parseChildren: shouldParse);

            IPage<IDomain, HtmlNode> competitionPage = continentCompetitionsSection[new Dictionary<string, string> { { "Title", chosenCompetitionLink.Title } }];
            var clubsSection = (ChildsSection<HtmlNode, ClubPage>)competitionPage["Competition - Clubs Section"];
            if (clubsSection == null)
            {
                throw new Exception("Parser was not able to continue as the section was not found.");
            }

            if (!clubsIndex.HasValue)
            {
                return competitionPage.Domain;
            }


            Link<HtmlNode, ClubPage> chosenClubLink = clubsSection.Children[clubsIndex.Value - 1];

            shouldParse = (!peek);
            clubsSection.Parse(new[] { chosenClubLink }, parseChildren: shouldParse);

            IPage<IDomain, HtmlNode> clubPage = clubsSection[new Dictionary<string, string> { { "Title", chosenClubLink.Title } }];
            var playersSection = (ChildsSamePageSection<Player, HtmlNode>)clubPage["Club - Players Section"];

            return clubPage.Domain;
        }

        private bool ContinentExists(int i1)
        {
            return Continents.ContainsKey(i1.ToString());
        }

        private void AddNewYearContinentIfDoesntExist(int year, int i1)
        {
            var key = GenerateKey(year, i1);
            if (!SeasonContinents.ContainsKey(key))
            {
                SeasonContinents.Add(key, (Continents[i1.ToString()], null));
            }
        }

        private (Link<HtmlNode, CompetitionPage> Link, ContinentPage Page) GetSeasonContinent(string key)
        {
            if (!SeasonContinents.ContainsKey(key))
            {
                throw new KeyNotFoundException(KEY_ERROR);
            }
            return SeasonContinents[key];
        }

        private string GenerateKey(int year, int i1)
        {
            return string.Format(KEY_PATTERN, year, i1);
        }
    }
}
