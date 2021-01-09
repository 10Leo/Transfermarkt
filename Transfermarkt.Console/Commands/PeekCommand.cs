﻿using LJMB.Command;
using System;
using System.Linq;
using Transfermarkt.Console.Arguments;
using Transfermarkt.Console.Options;
using Transfermarkt.Core.Service;

namespace Transfermarkt.Console
{
    public class PeekCommand : Command
    {
        public const string KEY = "f";
        public const string NAME = "Peek";

        public const string PEEK_ERROR_MSG = "Peek requires 1+ indexes passed to proccess.";
        public const string PEEK_NAME_OPTION_ERROR_MSG = "Peek requires the -i option.";
        public const string SEASON_ERROR_MSG = "Season was not defined.";

        public TMCommandProcessor TMContext
        {
            get
            {
                if (tmContext == null)
                {
                    tmContext = (TMCommandProcessor)Context;
                }
                return tmContext;
            }
        }
        public TMService TMService
        {
            get { return TMContext.TMService; }
        }
        public IOption Year
        {
            get
            {
                if (year == null)
                {
                    year = this[YearOption.NAME];
                }
                return year;
            }
        }
        public IOption Indexes
        {
            get
            {
                if (indexes == null)
                {
                    indexes = this[IndexesOption.NAME];
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
                    throw new Exception(SEASON_ERROR_MSG);
                }

                return int.Parse(((StringArgument)Year.Args.First()).Value);
            }
        }

        private TMCommandProcessor tmContext = null;
        private IOption year = null;
        private IOption indexes = null;

        public PeekCommand(IProcessor context)
        {
            this.Name = NAME.ToLower();
            this.AllowedAlias.Add(KEY);
            this.AllowedAlias.Add(NAME.ToLower());
            this.AllowedAlias.Add("fetch");
            this.Context = context;
            //this.Context.RegisterCommand(this);

            var y = new YearOption();
            //TODO: Peek command should not have any required options. If no Indexes are passed, fetch the entry page.
            var i = new IndexesOption
            {
                Required = true
            };
            this.RegisterOption(y);
            this.RegisterOption(i);
        }

        public override void Validate()
        {
            base.Validate();

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

                if (ind is Index1Argument)
                {
                    TMService.Parse(YearValue.Value, i1, peek: true);
                }
                else if (ind is Index2Argument)
                {
                    TMService.Parse(YearValue.Value, i1, i2, peek: true);
                }
                else if (ind is Index3Argument)
                {
                    TMService.Parse(YearValue.Value, i1, i2, i3, peek: true);
                }
            }

            TMContext.PrintOptions(YearValue);
        }
    }
}