using LJMB.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Transfermarkt.Console.Options
{
    /// <summary>
    /// -x <format> <save_path>
    /// </summary>
    public class ExportOption : Option
    {
        public const string NAME = "Export";
        public const string KEY = "x";

        private const string g1 = "Format";
        private const string g2 = "Path";

        public ExportOption()
        {
            Name = NAME;
            AllowedAlias = new HashSet<string> { KEY, NAME.ToLower() };
        }

        public override void Parse(string toParse)
        {
            var pattern = $@"(?<{g1}>\w+)(?:\s+(?<{g2}>.+))?";//"(?<{g1}>\w+)(?:\s+(?<{g2}>.+))?";

            Match splitArguments = Regex.Match(toParse, pattern);
            var splitArgumentss = Regex.Matches(toParse, pattern);

            string v1 = splitArguments.Groups[g1]?.Value?.Trim()?.ToUpperInvariant();
            string v2 = splitArguments.Groups[g2]?.Value?.Trim();

            if (string.IsNullOrEmpty(v1))
            {
                throw new Exception(ParseCommand.EXPORT_ARGUMENTS_NOT_FOUND_ERROR_MSG);
            }

            Args.Add(new String2Argument { Value = v1, Value2 = v2});
            Active = true;
        }
    }

    public class String2Argument : IArgument
    {
        public string Value { get; set; }
        public string Value2 { get; set; }
    }
}
