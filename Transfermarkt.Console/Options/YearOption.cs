using LJMB.Command;
using System;
using System.Collections.Generic;

namespace Transfermarkt.Console.Options
{
    public class YearOption : Option
    {
        public const string NAME = "Year";
        public const string KEY = "y";

        public YearOption()
        {
            Name = NAME;
            Usage = $"-{KEY} <y>";
            AllowedAlias = new HashSet<string> { KEY, NAME.ToLower() };
            Args = new HashSet<IArgument>(1);
        }

        protected override void OnParse(string toParse)
        {
            int y;
            try
            {
                y = int.Parse(toParse);

                var year = new StringArgument
                {
                    Value = y.ToString()
                };

                Args.Add(year);
            }
            catch (Exception)
            {
                //throw new Exception($"No argument passed to the {this.Name} option");
            }
        }

        public override void Validate()
        {
            base.Validate();

            if (!Active)
            {
                return;
            }

            if (Args == null || Args.Count == 0)
            {
                throw new Exception(string.Format(Exceptions.ARGUMENTS_NOT_FOUND_ERROR_MSG, this.Name));
            }
            if (Args.Count > 1)
            {
                throw new Exception(string.Format(Exceptions.TOO_MUCH_ARGUMENTS_ERROR_MSG, this.Name));
            }
        }
    }
}
