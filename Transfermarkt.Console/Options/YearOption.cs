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

        public override void Parse(string toParse)
        {
            int y;
            try
            {
                y = int.Parse(toParse);
            }
            catch (System.Exception)
            {

                throw new Exception($"No argument passed to the option {this.Name}");
            }

            var year = new StringArgument
            {
                Value = y.ToString()
            };

            Args.Add(year);

            Active = true;
        }
    }
}
