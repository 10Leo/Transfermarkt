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
using Transfermarkt.Core.Service;

namespace Transfermarkt.Console
{
    public class PeekCommand : Command
    {
        //public string ClubFileNameFormat { get; set; }
        //public string ContinentFileNameFormat { get; set; }
        //public string CompetitionFileNameFormat { get; set; }

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

        public TMService TMService
        {
            get { return TMContext.TMService; }
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

        public static readonly string NAME = "f";

        public PeekCommand(IContext context)
        {
            this.Name = "peek";
            this.AllowedAlias.Add("f");
            this.AllowedAlias.Add("fetch");
            this.AllowedAlias.Add("peek");
            this.Context = context;
            //this.Context.RegisterCommand(this);
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

                IDomain domain = null;
                if (ind is Index1Argument)
                {
                    domain = TMService.Parse(YearValue.Value, i1);
                }
                else if (ind is Index2Argument)
                {
                    domain = TMService.Parse(YearValue.Value, i1, i2);
                }
                else if (ind is Index3Argument)
                {
                    domain = TMService.Parse(YearValue.Value, i1, i2, i3);
                }
            }
        }
    }
}
