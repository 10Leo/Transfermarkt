using HtmlAgilityPack;
using LJMB.Command;
using Page.Scraper.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Pages;
using Transfermarkt.Core.Actors;

namespace Transfermarkt.Console
{
    public class FetchCommand : Command
    {
        public string ClubFileNameFormat { get; private set; }
        public string ContinentFileNameFormat { get; private set; }
        public string CompetitionFileNameFormat { get; private set; }
        public IContext Ctx { get; set; }
        public TMContext DescendantCtx { get { return (TMContext)Ctx; } set { DescendantCtx = value; } }

        public FetchCommand(IContext context)
        {
            this.Name = "fetch";
            this.Ctx = context;
            this.Ctx.RegisterCommand(this);
        }

        public override bool CanParse(string cmdToParse)
        {
            return cmdToParse == "f";
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
            IndexesParameterValue i = Parameters.FirstOrDefault(a => a.Cmd == OptionName.I).Val as IndexesParameterValue;

            // Check if a year was passed by the user as an argument. If not get the last passed one, or the current one, if one was not passed yet. 
            var yy = this[OptionName.Y];
            if (yy == null)
            {
                var y = new StringParameterValue
                {
                    Value = DescendantCtx.lastSelectedSeason
                };
                Parameters.Add((OptionName.Y, y));
            }

            DescendantCtx.lastSelectedSeason = ((StringParameterValue)this[OptionName.Y]).Value;

            foreach (IIndex ind in i.Indexes)
            {
                int i1 = 0;
                int i2 = 0;
                int i3 = 0;

                if (ind is Index1ParameterValue)
                {
                    i1 = (ind as Index1ParameterValue).Index1;
                }
                else if (ind is Index2ParameterValue)
                {
                    i1 = (ind as Index2ParameterValue).Index1;
                    i2 = (ind as Index2ParameterValue).Index2;
                }
                else if (ind is Index3ParameterValue)
                {
                    i1 = (ind as Index3ParameterValue).Index1;
                    i2 = (ind as Index3ParameterValue).Index2;
                    i3 = (ind as Index3ParameterValue).Index3;
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
                    Comm(i1 == 0 ? (int?)null : i1, i2 == 0 ? (int?)null : i2, i3 == 0 ? (int?)null : i3);
            }
        }

        private bool ContinentsP(int i1)
        {
            var year = ((StringParameterValue)this[OptionName.Y]).Value;

            var k = $"{year}.{i1}";

            if (!DescendantCtx.cont.ContainsKey(i1.ToString()))
            {
                return false;
            }

            if (!DescendantCtx.continent.ContainsKey(k))
            {
                DescendantCtx.continent.Add(k, (DescendantCtx.cont[i1.ToString()].L, null));
            }

            return true;
        }

        private bool Comm(int? i1, int? i2, int? i3)
        {
            var year = int.Parse(((StringParameterValue)this[OptionName.Y]).Value);

            var k = $"{year}.{i1}";

            if (!DescendantCtx.continent.ContainsKey(k))
            {
                return false;
            }
            (Link L, ContinentPage P) choice = DescendantCtx.continent[k];

            bool isFinal = !i2.HasValue && !i3.HasValue;

            if (choice.P == null || !choice.P.Connection.IsConnected)
            {
                choice.P = (ContinentPage)Activator.CreateInstance(typeof(ContinentPage), new HAPConnection(), DescendantCtx.Logger, year);
                //var c = Activator.CreateInstance<ContinentPage>();
                //c.Connection = new HAPConnection();
                DescendantCtx.continent[k] = choice;

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
