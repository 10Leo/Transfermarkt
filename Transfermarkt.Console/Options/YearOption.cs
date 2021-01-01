using LJMB.Command;
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
            AllowedAlias = new HashSet<string> { KEY, NAME.ToLower() };
            Args = new HashSet<IArgument>(1);
        }

        public override void Parse(string toParse)
        {
            var year = new StringArgument
            {
                Value = int.Parse(toParse).ToString()
            };

            Args.Add(year);

            Active = true;
        }
    }
}
