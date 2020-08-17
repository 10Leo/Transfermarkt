using LJMB.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Transfermarkt.Console.Options
{
    public class IndexesOption : IOption
    {
        private const string g1 = "I1";
        private const string g2 = "I2";
        private const string g3 = "I3";

        public string Name { get; set; } = "Indexes";
        public ISet<string> AllowedAlias { get; } = new HashSet<string> { "i" };
        public ISet<IArgument> Args { get; set; } = new HashSet<IArgument>();

        public void Parse(string value)
        {
            var pattern = $@"((?<{g1}>\d+)\.*(?<{g2}>\d*)\.*(?<{g3}>\d*))";

            MatchCollection splitArguments = Regex.Matches(value, pattern);

            foreach (Match argument in splitArguments)
            {
                var i = DetermineNumberOfIndexes(argument);
                Args.Add(i);
            }
        }

        private static IArgument DetermineNumberOfIndexes(Match argument)
        {
            var o1 = argument.Groups[g1].Value.Trim();
            var o2 = argument.Groups[g2].Value.Trim();
            var o3 = argument.Groups[g3].Value.Trim();

            if (string.IsNullOrEmpty(o1) || string.IsNullOrWhiteSpace(o1))
            {
                throw new Exception(ErrorMsg.ERROR_MSG_ARGS);
            }
            else if (string.IsNullOrEmpty(o2) || string.IsNullOrWhiteSpace(o2))
            {
                return new Index1Argument { Index1 = int.Parse(o1) };
            }
            else if (string.IsNullOrEmpty(o3) || string.IsNullOrWhiteSpace(o3))
            {
                return new Index2Argument { Index1 = int.Parse(o1), Index2 = int.Parse(o2) };
            }
            else
            {
                return new Index3Argument { Index1 = int.Parse(o1), Index2 = int.Parse(o2), Index3 = int.Parse(o3) };
            }
        }
    }

    public class Index1Argument : IArgument
    {
        public int Index1 { get; set; }
    }

    public class Index2Argument : IArgument
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
    }

    public class Index3Argument : IArgument
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int Index3 { get; set; }
    }
}
