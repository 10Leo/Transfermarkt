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

        public List<(Argument Cmd, IArgumentValue Val)> Args { get; set; } = new List<(Argument Cmd, IArgumentValue Val)>();

        public override string ToString()
        {
            string cmdToParse = $"{CommandType.ToString()}";
            if (Args?.Count > 0)
            {
                cmdToParse += $" {string.Join(" ", Args.Select(t => string.Format("{0}:{1}", t.Cmd, t.Val)))}";
            }

            return cmdToParse;
        }
    }

    public class Arg
    {
        public Argument Argument { get; set; }
        public IArgumentValue Value { get; set; }
    }

    public interface IArgumentValue
    {
    }

    public class StringArgumentValue : IArgumentValue
    {
        public string Value { get; set; }
    }

    public class Index1ArgumentValue : IArgumentValue
    {
        public List<int> Indexes { get; set; } = new List<int>();

    }

    public class Index2ArgumentValue : IArgumentValue
    {
        public List<(int Index1, int Index2)> Indexes { get; set; } = new List<(int Index1, int Index2)>();
    }

    public enum CommandType
    {
        F,
        P
    }

    public enum Argument
    {
        Y,
        O
    }
}
