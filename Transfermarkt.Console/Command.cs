using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Console
{
    public class Command
    {
        public IParameterValue this[ParameterName parameter]
        {
            get {
                if (Parameters.Any(p => p.Cmd == parameter))
                {
                    return Parameters.FirstOrDefault(p => p.Cmd == parameter).Val;
                }

                return null;
            }
        }

        public Action Action { get; set; }

        public List<(ParameterName Cmd, IParameterValue Val)> Parameters { get; set; } = new List<(ParameterName Cmd, IParameterValue Val)>();

        public override string ToString()
        {
            string cmdToParse = $"{Action.ToString()}";
            if (Parameters?.Count > 0)
            {
                cmdToParse += $" {string.Join(" ", Parameters.Select(t => string.Format("{0}:{1}", t.Cmd, t.Val)))}";
            }

            return cmdToParse;
        }
    }

    public class Parameter
    {
        public ParameterName Name { get; set; }
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

    // TODO: Reformat code so that these actions become classes with all the logic that pertains to them in themselves.
    public enum Action
    {
        /// <summary>
        /// Fetch links from page.
        /// </summary>
        F,
        /// <summary>
        /// Parse page.
        /// </summary>
        P,
        /// <summary>
        /// Exit.
        /// </summary>
        E
    }

    public enum ParameterName
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
}
