using LJMB.Command;
using Page.Scraper.Contracts;
using Page.Scraper.Exporter;
using System;
using System.Collections.Generic;
using System.Linq;
using Transfermarkt.Console.Arguments;
using Transfermarkt.Console.Options;
using Transfermarkt.Core;
using Transfermarkt.Core.Service;

namespace Transfermarkt.Console
{
    public class ParseCommand : Command
    {
        public const string KEY = "p";
        public const string NAME = "Parse";

        public const string PARSE_NAME_OPTION_ERROR_MSG = "Parse requires the -i option.";
        public const string PARSE_ERROR_MSG = "Parse requires 1+ indexes passed to proccess.";
        public const string KEY_ERROR_MSG = "Specified key doesn't exist.";
        public const string SEASON_ERROR_MSG = "Season was not defined.";
        public const string EXPORT_TYPE_NOT_FOUND_ERROR_MSG = "Export option {0} doesn't exist.";
        public const string EXPORT_ARGUMENTS_NOT_FOUND_ERROR_MSG = "Arguments not found in the Export option.";

        public string ClubFileNameFormat { get; set; }
        public string ContinentFileNameFormat { get; set; }
        public string CompetitionFileNameFormat { get; set; }

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
        public TMService TMService { get { return TMContext.TMService; } }
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
        public IOption Export
        {
            get
            {
                if (export == null)
                {
                    export = this[ExportOption.NAME];
                }
                return export;
            }
        }

        public IDictionary<ExportType, IExporter> Exporters { get; internal set; }

        private TMCommandProcessor tmContext = null;
        private IOption year = null;
        private IOption indexes = null;
        private IOption export = null;

        public ParseCommand(IProcessor context)
        {
            this.Name = NAME.ToLower();
            this.AllowedAlias.Add(KEY);
            this.AllowedAlias.Add(NAME.ToLower());
            this.Context = context;
            //this.Context.RegisterCommand(this);

            var y = new YearOption();
            var i = new IndexesOption
            {
                Required = true
            };
            var e = new ExportOption();
            this.RegisterOption(y);
            this.RegisterOption(i);
            this.RegisterOption(e);
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

            if (Export.Args.FirstOrDefault() is String2Argument arg)
            {
                ExportType? exportType;
                try
                {
                    exportType = (ExportType)Enum.Parse(typeof(ExportType), arg.Value);
                }
                catch (Exception)
                {
                    throw new KeyNotFoundException(string.Format(EXPORT_TYPE_NOT_FOUND_ERROR_MSG, arg.Value));
                }

                if (!Exporters.ContainsKey(exportType.Value))
                {
                    throw new KeyNotFoundException(string.Format(EXPORT_TYPE_NOT_FOUND_ERROR_MSG, arg.Value));
                }
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

                string template = string.Empty;
                IDomain domain = null;
                if (ind is Index1Argument)
                {
                    domain = TMService.Parse(YearValue.Value, i1);
                    template = ContinentFileNameFormat;
                }
                else if (ind is Index2Argument)
                {
                    domain = TMService.Parse(YearValue.Value, i1, i2);
                    template = CompetitionFileNameFormat;
                }
                else if (ind is Index3Argument)
                {
                    domain = TMService.Parse(YearValue.Value, i1, i2, i3);
                    template = ClubFileNameFormat;
                }

                if (Export != null)
                {
                    if (Export.Active && Export.Args.Count == 0)
                    {
                        throw new Exception(EXPORT_ARGUMENTS_NOT_FOUND_ERROR_MSG);
                    }

                    if (Export.Args.FirstOrDefault() is String2Argument arg)
                    {
                        var exportType = (ExportType)Enum.Parse(typeof(ExportType), arg.Value);
                        var exporter = Exporters[exportType];
                        exporter.Extract(domain, template);
                    }
                }
            }

            TMContext.PrintOptions(YearValue);
        }
    }
}
