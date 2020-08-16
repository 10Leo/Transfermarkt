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
        public string Name { get; set; }
        protected ISet<string> AllowedAlias { get; private set; } = new HashSet<string>();

        //public ISet<IOption> AllowedOptions { get; set; }
        public ISet<IOption> Options { get; set; } = new HashSet<IOption>();

        public IArgument this[string option]
        {
            get
            {
                var opt = Options.FirstOrDefault(o => o.Name == option);

                if (opt == null)
                {
                    return null;
                }

                return opt.Args.FirstOrDefault();
            }
        }

        //public IOption this[string option]
        //{
        //    get
        //    {
        //        return Options.FirstOrDefault(o => o.Name == option);
        //    }
        //}

        public void RegisterOption(IOption option)
        {
            if (Options.Contains(option))
            {
                throw new Exception("Option already registered.");
            }

            Options.Add(option);
        }

        public virtual bool CanParse(string cmdToParse)
        {
            return AllowedAlias.Contains(cmdToParse);
        }

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

                    var aa = a.Substring(1);

                    foreach (IOption option in Options)
                    {
                        if (option.AllowedAlias.Contains(aa))
                        {
                            option.Parse(v);
                            break;
                        }
                    }
                }
            }
        }

        public abstract void Validate();

        public abstract void Execute();

        public override string ToString()
        {
            string cmdToParse = $"{Name}";
            if (Options?.Count > 0)
            {
                cmdToParse += $" {string.Join(" ", Options.Select(t => string.Format("{0}", t)))}";
            }

            return cmdToParse;
        }
    }
}
