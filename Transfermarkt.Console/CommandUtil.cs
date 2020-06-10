using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Transfermarkt.Console
{
    public static class CommandUtil
    {
        public struct ErrorMsg
        {
            public static readonly string ERROR_MSG_CMD = "Command not specified.";
            public static readonly string ERROR_MSG_INTERPRET = "Error reading or interpreting supplied numbers.";
            public static readonly string ERROR_MSG_CMD_NOT_FOUND = "Command not found.";
            public static readonly string ERROR_MSG_NO_OPTIONS_PASSED = "No options passed.";
            public static readonly string ERROR_MSG_ARGS = "Args not specified.";
        }

        private static readonly List<string> AllowedYearStringCmd = new List<string> { "y", "year" };
        private static readonly List<string> AllowedObjStringCmd = new List<string> { "i" };

        public static Command ParseCommand(string line)
        {
            Command cmd = new Command();
            try
            {
                if (string.IsNullOrEmpty(line = line?.Trim()))
                {
                    throw new Exception(ErrorMsg.ERROR_MSG_CMD);
                }

                var cmdGroup = "CMD";
                var argsGroup = "Args";
                var argNameGroup = "ArgName";
                var argValueGroup = "ArgValue";

                var m = Regex.Matches(line, $@"^(?<{cmdGroup}>\w)\s*|(?<{argsGroup}>(?<{argNameGroup}>-\w+)\s+(?<{argValueGroup}>[^-]+))");

                if (m.Count > 0)
                {
                    var cm = m[0].Groups[cmdGroup];
                    if (cm == null || string.IsNullOrEmpty(cm.Value) || string.IsNullOrWhiteSpace(cm.Value))
                    {
                        throw new Exception(ErrorMsg.ERROR_MSG_CMD);
                    }

                    if (cm.Value.Trim().ToLowerInvariant() == "f")
                    {
                        cmd.CommandType = CommandType.F;
                    }
                    else if (cm.Value.Trim().ToLowerInvariant() == "p")
                    {
                        cmd.CommandType = CommandType.P;
                    }
                    else if (cm.Value.Trim().ToLowerInvariant() == "e")
                    {
                        cmd.CommandType = CommandType.E;
                        return cmd;
                    }
                    else
                    {
                        throw new Exception(ErrorMsg.ERROR_MSG_CMD);
                    }
                }

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

                        ParameterName? aa = ToArgument(a);
                        IParameterValue vv = ToValue(aa.Value, v);

                        cmd.Parameters.Add((aa.Value, vv));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ErrorMsg.ERROR_MSG_INTERPRET);
                System.Console.WriteLine(ex.Message);
                throw;
            }

            return cmd;
        }

        private static ParameterName? ToArgument(string a)
        {
            var aa = a.Substring(1);

            if (AllowedYearStringCmd.Contains(aa))
            {
                return ParameterName.Y;
            }
            if (AllowedObjStringCmd.Contains(aa))
            {
                return ParameterName.I;
            }

            return null;
        }

        private const string g1 = "I1";
        private const string g2 = "I2";
        private const string g3 = "I3";

        private static IParameterValue ToValue(ParameterName a, string v)
        {
            switch (a)
            {
                case ParameterName.Y:
                    var year = new StringParameterValue
                    {
                        Value = ParseYear(v).ToString()
                    };
                    return year;

                case ParameterName.I:
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
    }
}
