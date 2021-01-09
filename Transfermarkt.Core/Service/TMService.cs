using HtmlAgilityPack;
using LJMB.Logging;
using Page.Scraper.Contracts;
using System;
using System.Collections.Generic;
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
        public readonly IDictionary<int, ContinentsPage> SeasonContinents;

        public string BaseURL { get; set; }
        public ILogger Logger { get; set; }

        public TMService()
        {
            SeasonContinents = new Dictionary<int, ContinentsPage>();
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

            if (!ContinentExists(year, (ContinentCode)continentsIndex.Value))
            {
                throw new KeyNotFoundException(Message);
            }

            ContinentsPage continentsSeason = GetSeasonContinent(year);
            if (continentsSeason == null)
            {
                return null;
            }

            var chosenContinent = ((ChildsSection<HtmlNode, ContinentPage>)continentsSeason[ContinentsContinentsPageSection.SectionName]).Children[continentsIndex.Value - 1];
            if (chosenContinent.Page == null)
            {
                chosenContinent.Page = new ContinentPage(new HAPConnection(), Logger, year);
            }

            if (!chosenContinent.Page.Connection.IsConnected)
            {
                chosenContinent.Page.Connect($"{chosenContinent.Url}");
            }

            //TODO: create enum to hold parse and peek values and pass them to the Parse methods
            bool shouldParse = (!peek && !competitionsIndex.HasValue);
            chosenContinent.Page.Parse(parseChildren: shouldParse);

            if (!competitionsIndex.HasValue)
            {
                return chosenContinent.Page.Domain;
            }

            var continentCompetitionsSection = (ChildsSection<HtmlNode, CompetitionPage>)chosenContinent.Page["Continent - Competitions Section"];
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

        private bool ContinentExists(int year, ContinentCode i1)
        {
            if (!SeasonContinents.ContainsKey(year))
            {
                var continent = new ContinentsPage(new HAPConnection(), Logger, year);
                continent.Connect(BaseURL);
                continent.Parse(parseChildren: false);
                SeasonContinents.Add(year, continent);
            }

            var r = ((ChildsSection<HtmlAgilityPack.HtmlNode, ContinentPage>)SeasonContinents[year][ContinentsContinentsPageSection.SectionName]).Children[((int)i1) - 1];
            return r != null;
        }

        private void AddNewYearContinentIfDoesntExist(int year, ContinentCode i1)
        {
            if (!SeasonContinents.ContainsKey(year))
            {
                var continent = new ContinentsPage(new HAPConnection(), Logger, year);
                continent.Parse(parseChildren: false);
                SeasonContinents.Add(year, continent);
            }
        }

        private ContinentsPage GetSeasonContinent(int year)
        {
            if (!SeasonContinents.ContainsKey(year))
            {
                throw new KeyNotFoundException(KEY_ERROR);
            }

            return SeasonContinents[year];
        }
    }
}
