using LJMB.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Console.Options
{
    public class YearOption : IOption
    {
        public string Name { get; set; } = "Year";
        public ISet<string> AllowedAlias { get; } = new HashSet<string> { "y", "year" };
        public ISet<IArgument> Args { get; set; } = new HashSet<IArgument>();

        public void Parse(string value)
        {
            var year = new StringParameterArgument
            {
                Value = ParseYear(value).ToString()
            };

            Args.Add(year);
        }

        private static int ParseYear(string yearCmd)
        {
            return int.Parse(yearCmd);
        }
    }
}
