using HtmlAgilityPack;
using LJMB.Command;
using Page.Scraper.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Transfermarkt.Console.Arguments;
using Transfermarkt.Console.Options;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Pages;

namespace Transfermarkt.Console
{
    public class PeekCommand : Command
    {
        public string ClubFileNameFormat { get; set; }
        public string ContinentFileNameFormat { get; set; }
        public string CompetitionFileNameFormat { get; set; }

        private TMContext tmContext = null;
        public TMContext TMContext
        {
            get
            {
                if (tmContext == null)
                {
                    tmContext = (TMContext)Context;
                }
                return tmContext;
            }
        }

        private IOption year = null;
        public IOption Year
        {
            get
            {
                if (year == null)
                {
                    year = this["Year"];
                }
                return year;
            }
        }

        private IOption indexes = null;
        public IOption Indexes
        {
            get
            {
                if (indexes == null)
                {
                    indexes = this["Indexes"];
                }
                return indexes;
            }
        }

        public int? YearValue
        {
            get
            {
                if (Year == null || Year.Args == null || Year.Args.Count == 0)
                {
                    throw new Exception("Season was not defined.");
                }

                return int.Parse(((StringArgument)Year.Args.First()).Value);
            }
        }

        public static readonly string PEEK_ERROR_MSG = "Peek requires 1+ indexes passed to proccess.";
        public static readonly string PEEK_NAME_OPTION = "Peek requires the -i option.";
        public static readonly string KEY_ERROR = "Specified key doesn't exist.";

        public PeekCommand(IContext context)
        {
            this.Name = "peek";
            this.AllowedAlias.Add("f");
            this.AllowedAlias.Add("fetch");
            this.AllowedAlias.Add("peek");
            this.Context = context;
            this.Context.RegisterCommand(this);
            this.RegisterOption(new YearOption());
            this.RegisterOption(new IndexesOption());
        }

        public override void Validate()
        {
            if (Year != null)
            {
                // Check if a year was passed by the user as an argument. If not get the last passed one, or the current one, if one was not passed yet. 
                if (Year.Args.Count == 0)
                {
                    var y = new StringArgument
                    {
                        Value = TMContext.LastSelectedSeason
                    };
                    Year.Args.Add(y);
                }
            }
            TMContext.LastSelectedSeason = YearValue.ToString();

            if (Indexes == null)
            {
                throw new ArgumentException(PEEK_NAME_OPTION);
            }
            if (Indexes.Args.Count == 0)
            {
                throw new ArgumentException(PEEK_ERROR_MSG);
            }
        }

        public override void Execute()
        {
            Process();
        }

        private void Process()
        {
            foreach (IArgument ind in Indexes.Args)
            {
                (int i1, int i2, int i3) = ind.GetIndexes();

                if (!ContinentExists(i1))
                {
                    continue;
                }
                AddNewYearContinentIfDoesntExist(i1);

                bool proceed = true;
                if (proceed && i1 != 0)
                {
                    proceed = ProcessIndex1(i1, ind is Index1Argument);
                }
                if (proceed && i2 != 0)
                {
                    proceed = ProcessIndex2(i1, i2, ind is Index2Argument);
                }
                if (proceed && i3 != 0)
                {
                    proceed = ProcessIndex3(i1, i2, i3, ind is Index3Argument);
                }

                //if (proceed && i1 != 0)
                //    ProccessCommand(i1 == 0 ? (int?)null : i1, i2 == 0 ? (int?)null : i2, i3 == 0 ? (int?)null : i3);
            }
        }

        private bool ContinentExists(int i1)
        {
            return TMContext.Continents.ContainsKey(i1.ToString());
        }

        private void AddNewYearContinentIfDoesntExist(int i1)
        {
            var key = GenerateKey(i1);
            if (!TMContext.Continent.ContainsKey(key))
            {
                TMContext.Continent.Add(key, (TMContext.Continents[i1.ToString()].L, null));
            }
        }

        private (Link<HtmlNode, CompetitionPage> L, ContinentPage P) GetSeasonContinent(string key)
        {
            if (!TMContext.Continent.ContainsKey(key))
            {
                throw new KeyNotFoundException(KEY_ERROR);
            }
            return TMContext.Continent[key];
        }

        private bool ProcessIndex1(int i1, bool isFinal)
        {
            var key = GenerateKey(i1);

            (Link<HtmlNode, CompetitionPage> L, ContinentPage P) choice = GetSeasonContinent(key);
            if (choice.L == null)
            {
                return false;
            }
            if (choice.P == null)
            {
                choice.P = new ContinentPage(new HAPConnection(), TMContext.Logger, YearValue);
                TMContext.Continent[key] = choice;
            }

            if (!choice.P.Connection.IsConnected)
            {
                choice.P.Connect(choice.L.Url);
            }

            //if (choice.P.ParseLevel == ParseLevel.NotYet)
            {
                choice.P.Parse(parseChildren: false);
            }

            var continentCompetitionsSection = (ChildsSection<HtmlNode, CompetitionPage>)choice.P["Continent - Competitions Section"];
            if (continentCompetitionsSection != null)
            {
                TMContext.PresentOptions(continentCompetitionsSection.Children, $"{i1}", 1);
            }

            return !isFinal;
        }

        private bool ProcessIndex2(int i1, int i2, bool isFinal)
        {
            var key = GenerateKey(i1);

            (Link<HtmlNode, CompetitionPage> L, ContinentPage P) = GetSeasonContinent(key);

            var continentCompetitionsSection = (ChildsSection<HtmlNode, CompetitionPage>)P["Continent - Competitions Section"];

            Link<HtmlNode, CompetitionPage> chosenCompetitionLink = continentCompetitionsSection.Children[i2 - 1];
            //if (continentCompetitionsSection.ParseLevel == ParseLevel.NotYet)
            {
                continentCompetitionsSection.Parse(new[] { chosenCompetitionLink }, parseChildren: false);
            }

            IPage<IDomain, HtmlNode> competitionPage = continentCompetitionsSection[new Dictionary<string, string> { { "Title", chosenCompetitionLink.Title } }];
            var clubsSection = (ChildsSection<HtmlNode, ClubPage>)competitionPage["Competition - Clubs Section"];
            if (clubsSection != null)
            {
                System.Console.WriteLine();
                TMContext.PresentOptions(clubsSection.Children, $"{i1}.{i2}", 2);
            }

            return !isFinal;
        }

        private bool ProcessIndex3(int i1, int i2, int i3, bool isFinal)
        {
            var key = GenerateKey(i1);

            (Link<HtmlNode, CompetitionPage> L, ContinentPage P) = GetSeasonContinent(key);

            var continentCompetitionsSection = (ChildsSection<HtmlNode, CompetitionPage>)P["Continent - Competitions Section"];

            Link<HtmlNode, CompetitionPage> chosenCompetitionLink = continentCompetitionsSection.Children[i2 - 1];

            IPage<IDomain, HtmlNode> competitionPage = continentCompetitionsSection[new Dictionary<string, string> { { "Title", chosenCompetitionLink.Title } }];
            var clubsSection = (ChildsSection<HtmlNode, ClubPage>)competitionPage["Competition - Clubs Section"];

            Link<HtmlNode, ClubPage> chosenClubLink = clubsSection.Children[i3 - 1];
            //if (clubsSection.ParseLevel == ParseLevel.NotYet)
            {
                clubsSection.Parse(new[] { chosenClubLink }, parseChildren: false);
            }

            IPage<IDomain, HtmlNode> clubPage = clubsSection[new Dictionary<string, string> { { "Title", chosenClubLink.Title } }];
            var playersSection = (ChildsSamePageSection<Player, HtmlNode>)clubPage["Club - Players Section"];

            return !isFinal;
        }

        private string GenerateKey(int i1)
        {
            return $"{YearValue}.{i1}";
        }
    }
}
