using LJMB.Command;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transfermarkt.Console.Arguments;

namespace Transfermarkt.Console.Options
{
    public class IndexesOption : IOption
    {
        private const string g1 = "I1";
        private const string g2 = "I2";
        private const string g3 = "I3";

        public string Name { get; } = "Indexes";
        public ISet<string> AllowedAlias { get; } = new HashSet<string> { "i" };
        public ISet<IArgument> Args { get; } = new HashSet<IArgument>();

        public void Parse(string toParse)
        {
            var pattern = $@"((?<{g1}>\d+)\.*(?<{g2}>\d*)\.*(?<{g3}>\d*))";

            MatchCollection splitArguments = Regex.Matches(toParse, pattern);

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
}
