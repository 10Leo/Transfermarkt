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
        private static readonly List<string> AllowedObjStringCmd = new List<string> { "o" };

        public static Command ParseCommand(string line)
        {
            Command cmd = new Command();
            try
            {
                if (string.IsNullOrEmpty(line = line?.Trim()))
                {
                    throw new Exception(ErrorMsg.ERROR_MSG_CMD);
                }

                var m = Regex.Match(line, @"^(?<CMD>[\w\-]+)\s+(?<Args>.*)");
                var cm = m.Groups["CMD"];
                var arguments = m.Groups["Args"];

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
                else
                {
                    throw new Exception(ErrorMsg.ERROR_MSG_CMD);
                }

                if (arguments == null || string.IsNullOrEmpty(arguments.Value) || string.IsNullOrWhiteSpace(arguments.Value))
                {
                    throw new Exception(ErrorMsg.ERROR_MSG_CMD);
                }

                MatchCollection splitArguments = Regex.Matches(arguments.Value, @"((?<Arg>-[^-\s]+)\s+(?<Value>[^-]+))");
                foreach (Match argument in splitArguments)
                {
                    string a = argument.Groups["Arg"]?.Value?.Trim()?.ToLowerInvariant();
                    string v = argument.Groups["Value"]?.Value?.Trim();

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
                return ParameterName.O;
            }

            return null;
        }

        private static IParameterValue ToValue(ParameterName a, string v)
        {
            switch (a)
            {
                case ParameterName.Y:
                    var year = new StringParameterValue();
                    year.Value = ParseYear(v).ToString();
                    return year;
                case ParameterName.O:
                    var g1 = "I1";
                    var g2 = "I2";
                    var g3 = "I3";

                    MatchCollection splitArguments = Regex.Matches(v, $@"((?<{g1}>\d+)\.*(?<{g2}>\d*)\.*(?<{g3}>\d*))");

                    IParameterValue indexes = null;
                    int nIndexes = 0;

                    var structure = splitArguments.OfType<Match>().FirstOrDefault();
                    var o1 = structure.Groups[g1].Value.Trim();
                    var o2 = structure.Groups[g2].Value.Trim();
                    var o3 = structure.Groups[g3].Value.Trim();

                    if (string.IsNullOrEmpty(o1) || string.IsNullOrWhiteSpace(o1))
                    {
                        throw new Exception(ErrorMsg.ERROR_MSG_ARGS);
                    }
                    else if (string.IsNullOrEmpty(o2) || string.IsNullOrWhiteSpace(o2))
                    {
                        nIndexes = 1;
                        indexes = new Index1ParameterValue();
                    }
                    else if (string.IsNullOrEmpty(o3) || string.IsNullOrWhiteSpace(o3))
                    {
                        nIndexes = 2;
                        indexes = new Index2ParameterValue();
                    }
                    else
                    {
                        nIndexes = 3;
                        //indexes = new Index3ArgumentValue();
                    }

                    foreach (Match argument in splitArguments)
                    {
                        var a1 = argument.Groups[g1].Value.Trim();
                        var a2 = argument.Groups[g2].Value.Trim();
                        var a3 = argument.Groups[g3].Value.Trim();

                        if (nIndexes == 1)
                        {
                            ((Index1ParameterValue)indexes).Indexes.Add(int.Parse(a1));
                        }
                        else if (nIndexes == 2)
                        {
                            ((Index2ParameterValue)indexes).Indexes.Add((int.Parse(a1), int.Parse(a2)));
                        }
                    }

                    return indexes;
                default:
                    break;
            }

            return null;
        }

        private static int ParseYear(string yearCmd)
        {
            return int.Parse(yearCmd);
        }
    }
}
