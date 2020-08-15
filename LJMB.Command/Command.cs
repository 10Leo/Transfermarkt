using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LJMB.Command
{
    public abstract class Command : ICommand
    {
        private static readonly List<string> AllowedYearStringCmd = new List<string> { "y", "year" };
        private static readonly List<string> AllowedObjStringCmd = new List<string> { "i" };

        private const string g1 = "I1";
        private const string g2 = "I2";
        private const string g3 = "I3";

        public string Name { get; set; }
        public List<(OptionName Cmd, IParameterValue Val)> Parameters { get; set; } = new List<(OptionName Cmd, IParameterValue Val)>();

        public IParameterValue this[OptionName parameter]
        {
            get
            {
                if (Parameters.Any(p => p.Cmd == parameter))
                {
                    return Parameters.FirstOrDefault(p => p.Cmd == parameter).Val;
                }

                return null;
            }
        }

        public abstract bool CanParse(string cmdToParse);

        public virtual void Parse(string completeCmdToParse)
        {
            var cmdGroup = "CMD";
            var argsGroup = "Args";
            var argNameGroup = "ArgName";
            var argValueGroup = "ArgValue";

            var m = Regex.Matches(completeCmdToParse, $@"^(?<{cmdGroup}>\w)\s*|(?<{argsGroup}>(?<{argNameGroup}>-\w+)\s+(?<{argValueGroup}>[^-]+))");

            if (m.Count > 1)
            {
                for (int i = 1; i < m.Count; i++)
                {
                    var argument = m[i];

                    string a = argument.Groups[argNameGroup]?.Value?.Trim()?.ToLowerInvariant();
                    string v = argument.Groups[argValueGroup]?.Value?.Trim();

                    if (a == null || string.IsNullOrEmpty(a) || string.IsNullOrWhiteSpace(a))
                    {
                        throw new Exception(ErrorMsg.ERROR_MSG_ARGS);
                    }

                    if (v == null || string.IsNullOrEmpty(v) || string.IsNullOrWhiteSpace(v))
                    {
                        throw new Exception(ErrorMsg.ERROR_MSG_ARGS);
                    }

                    OptionName? aa = ToArgument(a);
                    IParameterValue vv = ToValue(aa.Value, v);

                    this.Parameters.Add((aa.Value, vv));
                }
            }
        }

        public abstract void Validate();

        public abstract void Execute();

        protected static OptionName? ToArgument(string a)
        {
            var aa = a.Substring(1);

            if (AllowedYearStringCmd.Contains(aa))
            {
                return OptionName.Y;
            }
            if (AllowedObjStringCmd.Contains(aa))
            {
                return OptionName.I;
            }

            return null;
        }

        protected static IParameterValue ToValue(OptionName a, string v)
        {
            switch (a)
            {
                case OptionName.Y:
                    var year = new StringParameterValue
                    {
                        Value = ParseYear(v).ToString()
                    };
                    return year;

                case OptionName.I:
                    var g1 = "I1";
                    var g2 = "I2";
                    var g3 = "I3";
                    var pattern = $@"((?<{g1}>\d+)\.*(?<{g2}>\d*)\.*(?<{g3}>\d*))";

                    MatchCollection splitArguments = Regex.Matches(v, pattern);

                    IndexesParameterValue indexes = new IndexesParameterValue();

                    foreach (Match argument in splitArguments)
                    {
                        var i = DetermineNumberOfIndexes(argument);
                        indexes.Indexes.Add(i);
                    }

                    return indexes;
                default:
                    break;
            }

            return null;
        }

        private static IIndex DetermineNumberOfIndexes(Match argument)
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
                return new Index1ParameterValue { Index1 = int.Parse(o1) };
            }
            else if (string.IsNullOrEmpty(o3) || string.IsNullOrWhiteSpace(o3))
            {
                return new Index2ParameterValue { Index1 = int.Parse(o1), Index2 = int.Parse(o2) };
            }
            else
            {
                return new Index3ParameterValue { Index1 = int.Parse(o1), Index2 = int.Parse(o2), Index3 = int.Parse(o3) };
            }
        }

        private static int ParseYear(string yearCmd)
        {
            return int.Parse(yearCmd);
        }

        public override string ToString()
        {
            string cmdToParse = $"{Name}";
            if (Parameters?.Count > 0)
            {
                cmdToParse += $" {string.Join(" ", Parameters.Select(t => string.Format("{0}:{1}", t.Cmd, t.Val)))}";
            }

            return cmdToParse;
        }
    }

    public class Option
    {
        public OptionName Name { get; set; }
        public IParameterValue Value { get; set; }
    }

    public interface IParameterValue
    {
    }

    public class StringParameterValue : IParameterValue
    {
        public string Value { get; set; }
    }
    public class IndexesParameterValue : IParameterValue
    {
        public List<IIndex> Indexes { get; set; } = new List<IIndex>();
    }

    public interface IIndex
    {
    }

    public class Index1ParameterValue : IIndex
    {
        public int Index1 { get; set; }
    }

    public class Index2ParameterValue : IIndex
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
    }

    public class Index3ParameterValue : IIndex
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int Index3 { get; set; }
    }

    public enum OptionName
    {
        /// <summary>
        /// Year.
        /// </summary>
        Y,
        /// <summary>
        /// Page(s) indexes to fetch/parse.
        /// </summary>
        I
    }

    public struct ErrorMsg
    {
        public static readonly string ERROR_MSG_CMD = "Command not specified.";
        public static readonly string ERROR_MSG_INTERPRET = "Error reading or interpreting chosen option.";//"Error reading or interpreting supplied numbers.";
        public static readonly string ERROR_MSG_CMD_NOT_FOUND = "Command not found.";
        public static readonly string ERROR_MSG_NO_OPTIONS_PASSED = "No options passed.";
        public static readonly string ERROR_MSG_ARGS = "Args not specified.";
    }
}
