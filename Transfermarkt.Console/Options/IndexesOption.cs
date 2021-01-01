using LJMB.Command;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transfermarkt.Console.Arguments;

namespace Transfermarkt.Console.Options
{
    public class IndexesOption : Option
    {
        public const string NAME = "Indexes";
        public const string KEY = "i";

        private const string g1 = "I1";
        private const string g2 = "I2";
        private const string g3 = "I3";

        public IndexesOption()
        {
            Name = NAME;
            AllowedAlias = new HashSet<string> { KEY, NAME.ToLower() };
            Usage = $"-{KEY} <i(.i(.i))> (<i(.i(.i))>)";
        }

        public override void Parse(string toParse)
        {
            Active = true;

            var pattern = $@"((?<{g1}>\d+)\.*(?<{g2}>\d*)\.*(?<{g3}>\d*))";

            MatchCollection splitArguments = Regex.Matches(toParse, pattern);

            //if (splitArguments == null || splitArguments.Count == 0)
            //{
            //    throw new Exception(ParseCommand.PARSE_ERROR_MSG);
            //}

            foreach (Match argument in splitArguments)
            {
                var i = DetermineNumberOfIndexes(argument);
                Args.Add(i);
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
