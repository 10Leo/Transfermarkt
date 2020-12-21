using LJMB.Command;
using System.Collections.Generic;

namespace Transfermarkt.Console.Options
{
    public class YearOption : IOption
    {
        public const string Key = "Year";

        public string Name { get; } = "Year";
        public ISet<string> AllowedAlias { get; } = new HashSet<string> { "y", "year" };
        public ISet<IArgument> Args { get; } = new HashSet<IArgument>(1);

        public void Parse(string toParse)
        {
            var year = new StringArgument
            {
                Value = int.Parse(toParse).ToString()
            };

            Args.Add(year);
        }

        public void Reset()
        {
            Args.Clear();
        }
    }
}
