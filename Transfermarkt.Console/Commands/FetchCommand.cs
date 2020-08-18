using HtmlAgilityPack;
using LJMB.Command;
using Page.Scraper.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Pages;
using Transfermarkt.Core.Actors;
using Transfermarkt.Console.Arguments;

namespace Transfermarkt.Console
{
    public class FetchCommand : Command
    {
        public string ClubFileNameFormat { get; private set; }
        public string ContinentFileNameFormat { get; private set; }
        public string CompetitionFileNameFormat { get; private set; }
        public TMContext TMContext { get { return (TMContext)Context; } set { TMContext = value; } }

        public FetchCommand(IContext context)
        {
            this.Name = "fetch";
            this.AllowedAlias.Add("f");
            this.AllowedAlias.Add("fetch");
            this.Context = context;
            this.Context.RegisterCommand(this);
        }

        public override void Validate()
        {
        }

        public override void Execute()
        {
            Proccess();
        }

        private void Proccess()
        {
            IOption idx = this["Indexes"];
            IOption yy = this["Year"];

            // Check if a year was passed by the user as an argument. If not get the last passed one, or the current one, if one was not passed yet. 
            if (yy.Args.Count == 0)
            {
                var y = new StringArgument
                {
                    Value = TMContext.lastSelectedSeason
                };
                yy.Args.Add(y);
            }

            TMContext.lastSelectedSeason = ((StringArgument)yy.Args.First()).Value;

            foreach (IArgument ind in idx.Args)
            {
                int i1 = 0;
                int i2 = 0;
                int i3 = 0;

                if (ind is Index1Argument)
                {
                    i1 = (ind as Index1Argument).Index1;
                }
                else if (ind is Index2Argument)
                {
                    i1 = (ind as Index2Argument).Index1;
                    i2 = (ind as Index2Argument).Index2;
                }
                else if (ind is Index3Argument)
                {
                    i1 = (ind as Index3Argument).Index1;
                    i2 = (ind as Index3Argument).Index2;
                    i3 = (ind as Index3Argument).Index3;
                }

                bool proceed = ContinentsP(i1);
                //if (proceed && i1 != 0)
                //{
                //    proceed = ContinentP(cmd, $"{i1.ToString()}", ind is Index1ParameterValue);
                //}
                //if (proceed && i2 != 0)
                //{
                //    proceed = CompetitionP(cmd, $"{i1.ToString()}.{i2.ToString()}", i2, i3, ind is Index2ParameterValue);
                //}
                //if (proceed && i3 != 0)
                //{
                //    //proceed = ClubP(cmd, $"{i1.ToString()}.{i2.ToString()}.{i3.ToString()}", ind is Index3ParameterValue);
                //}

                if (proceed && i1 != 0)
                    ProccessCommand(i1 == 0 ? (int?)null : i1, i2 == 0 ? (int?)null : i2, i3 == 0 ? (int?)null : i3);
            }
        }

        private bool ContinentsP(int i1)
        {
            var year = ((StringArgument)this["Year"].Args.First()).Value;

            var k = $"{year}.{i1}";

            if (!TMContext.cont.ContainsKey(i1.ToString()))
            {
                return false;
            }

            if (!TMContext.continent.ContainsKey(k))
            {
                TMContext.continent.Add(k, (TMContext.cont[i1.ToString()].L, null));
            }

            return true;
        }

        private bool ProccessCommand(int? i1, int? i2, int? i3)
        {
            var year = int.Parse(((StringArgument)this["Year"].Args.First()).Value);

            var k = $"{year}.{i1}";

            if (!TMContext.continent.ContainsKey(k))
            {
                return false;
            }
            (Link L, ContinentPage P) choice = TMContext.continent[k];

            bool isFinal = !i2.HasValue && !i3.HasValue;

            if (choice.P == null || !choice.P.Connection.IsConnected)
            {
                choice.P = (ContinentPage)Activator.CreateInstance(typeof(ContinentPage), new HAPConnection(), TMContext.Logger, year);
                //var c = Activator.CreateInstance<ContinentPage>();
                //c.Connection = new HAPConnection();
                TMContext.continent[k] = choice;

                choice.P.Connect(choice.L.Url);

                // do a fetch or a parse according to conditions.
                choice.P.Parse(parseChildren: false);
            }

            var continentCompetitionsSection = (ChildsSection<HtmlNode, CompetitionPage>)choice.P["Continent - Competitions Section"];
            if (continentCompetitionsSection != null)
            {
                System.Console.WriteLine();
                TMContext.PresentOptions(continentCompetitionsSection.Children, $"{i1}", 1);
            }


            if (isFinal || !i2.HasValue)
            {
                return true;
            }


            isFinal = !i3.HasValue;

            Link chosenCompetitionLink = continentCompetitionsSection.Children[i2.Value - 1];
            continentCompetitionsSection.Parse(new[] { chosenCompetitionLink }, parseChildren: false);

            IPage<IDomain, HtmlNode> competitionPage = continentCompetitionsSection[new Dictionary<string, string> { { "Title", chosenCompetitionLink.Title } }];
            var clubsSection = (ChildsSection<HtmlNode, ClubPage>)competitionPage["Competition - Clubs Section"];
            if (clubsSection != null)
            {
                System.Console.WriteLine();
                TMContext.PresentOptions(clubsSection.Children, $"{i1}.{i2}", 2);
            }


            if (isFinal || !i3.HasValue)
            {
                return true;
            }


            isFinal = true;

            Link chosenClubLink = clubsSection.Children[i3.Value - 1];
            clubsSection.Parse(new[] { chosenClubLink }, true);

            IPage<IDomain, HtmlNode> clubPage = clubsSection[new Dictionary<string, string> { { "Title", chosenClubLink.Title } }];
            var playersSection = (ChildsSamePageSection<Player, HtmlNode>)clubPage["Club - Players Section"];

            //if (isFinal)
            //{
            //    Context.exporter.Extract(clubPage.Domain, ClubFileNameFormat);
            //}

            return true;
        }
    }
}
