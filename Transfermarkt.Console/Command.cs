using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Console
{
    public class Command
    {
        public CommandType CommandType { get; set; }

        public List<(ExtraCommand Cmd, string Val)> ExtraArgs { get; set; } = new List<(ExtraCommand Cmd, string Val)>();

        public List<(int Index1, int Index2)> Options { get; set; } = new List<(int Index1, int Index2)>();

        public override string ToString()
        {
            string cmdToParse = $"{CommandType.ToString()}";
            if (ExtraArgs?.Count > 0)
            {
                cmdToParse += $" {string.Join(" ", ExtraArgs.Select(t => string.Format("{0}:{1}", t.Cmd, t.Val)))}";
            }
            if (Options?.Count > 0)
            {
                cmdToParse += $" {string.Join(" ", Options.Select(t => string.Format("{0}.{1}", t.Index1, t.Index2)))}";
            }

            return cmdToParse;
        }
    }

    public enum CommandType
    {
        F,
        P
    }

    public enum ExtraCommand
    {
        Y
    }
}
