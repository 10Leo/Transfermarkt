using LJMB.Command;
using System.Collections.Generic;

namespace Transfermarkt.Console.Options
{
    public class YearOption : Option
    {
        public const string KEY = "Year";
        public const string NAME = "y";

        public YearOption()
        {
            Name = KEY;
            AllowedAlias = new HashSet<string> { NAME, KEY.ToLower() };
            Args = new HashSet<IArgument>(1);
        }

        public override void Parse(string toParse)
        {
            var year = new StringArgument
            {
                Value = int.Parse(toParse).ToString()
            };

            Args.Add(year);
        }
    }
}
