using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LJMB.Command
{
    public abstract class Command : ICommand
    {
        //TODO: create additional command to list all registered commands
        protected ISet<string> AllowedAlias { get; private set; } = new HashSet<string>();

        public string Name { get; set; }
        public IProcessor Context { get; set; }
        public ISet<IOption> Options { get; } = new HashSet<IOption>();

        public IOption this[string option]
        {
            get
            {
                return Options.FirstOrDefault(o => o.Name == option);
            }
        }

        public virtual bool CanParse(string toParse)
        {
            return AllowedAlias.Contains(toParse);
        }

        public virtual void Parse(string toParse)
        {
            var cmdGroup = "CMD";
            var argsGroup = "Args";
            var argNameGroup = "ArgName";
            var argValueGroup = "ArgValue";

            MatchCollection m = Regex.Matches(toParse, $@"^(?<{cmdGroup}>\w)\s*|(?<{argsGroup}>(?<{argNameGroup}>-\w+)\s+(?<{argValueGroup}>[^-]+))");

            if (m.Count > 1)
            {
                for (int i = 1; i < m.Count; i++)
                {
                    Match argument = m[i];

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

            Validate();
        }

        public virtual void Validate() { }

        public abstract void Execute();

        protected void RegisterOption(IOption option)
        {
            if (Options.Contains(option))
            {
                throw new Exception("Option already registered.");
            }

            Options.Add(option);
        }

        public void Reset()
        {
            Options.ToList().ForEach(o => o.Reset());
        }

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
