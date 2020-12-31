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
            var pattern = $@"((?<{g1}>\w+) +(?<{g2}>.*))";

            Match splitArguments = Regex.Match(toParse, pattern);

            string v1 = splitArguments.Groups[g1]?.Value?.Trim()?.ToLowerInvariant();
            string v2 = splitArguments.Groups[g2]?.Value?.Trim();

            Args.Add(new String2Argument { Value = v1, Value2 = v2});
        }
    }

    public class String2Argument : IArgument
    {
        public string Value { get; set; }
        public string Value2 { get; set; }
    }
}
