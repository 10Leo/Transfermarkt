using LJMB.Command;
using System.Collections.Generic;

namespace Transfermarkt.Console.Options
{
    public class YearOption : IOption
    {
        public string Name { get; } = "Year";
        public ISet<string> AllowedAlias { get; } = new HashSet<string> { "y", "year" };
        public ISet<IArgument> Args { get; } = new HashSet<IArgument>();

        public void Parse(string toParse)
        {
            var year = new StringArgument
            {
                Value = ParseYear(toParse).ToString()
            };

            Args.Add(year);
        }

        private static int ParseYear(string yearCmd)
        {
            return int.Parse(yearCmd);
        }
    }
}
