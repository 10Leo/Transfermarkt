﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LJMB.Command
{
    public abstract class Command : ICommand
    {
        protected ISet<string> AllowedAlias { get; private set; } = new HashSet<string>();

        public string Name { get; set; }
        public string Usage
        {
            get
            {
                var req = string.Join(" ", Options.Where(o => o.Required).OrderBy(o => o.Name).Select(o => o.Usage));
                var nreq = string.Join(" ", Options.Where(o => !o.Required).OrderBy(o => o.Name).Select(o => o.Usage));
                return $"{Name} {req} {nreq}";
            }
        }

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
            var cmdGroup = "Command";
            var optionAndArgsGroup = "OptionsAndArgs";
            var optionKeyGroup = "OptionKey";
            var argValueGroup = "ArgValue";

            MatchCollection cmdOrOptionsMatches = Regex.Matches(toParse, $@"^(?<{cmdGroup}>\w)\s*|(?<{optionAndArgsGroup}>(?<{optionKeyGroup}>-\w+)(\s+(?<{argValueGroup}>[^-]+))*)");

            if (cmdOrOptionsMatches.Count > 1)
            {
                for (int i = 1; i < cmdOrOptionsMatches.Count; i++)
                {
                    Match optionAndArgToParseMatch = cmdOrOptionsMatches[i];

                    string opt = optionAndArgToParseMatch.Groups[optionKeyGroup]?.Value?.Trim()?.ToLowerInvariant();
                    string args = optionAndArgToParseMatch.Groups[argValueGroup]?.Value?.Trim();

                    if (string.IsNullOrEmpty(opt) || string.IsNullOrWhiteSpace(opt))
                    {
                        throw new Exception(ErrorMsg.ERROR_MSG_ARGS);
                    }

                    var optWithoutHiphen = opt.Substring(1);

                    foreach (IOption option in Options)
                    {
                        if (option.AllowedAlias.Contains(optWithoutHiphen))
                        {
                            option.Parse(args);
                            break;
                        }
                    }
                }
            }

            Validate();
        }

        public virtual void Validate() {
            foreach (var option in Options)
            {
                if (option.Required)
                {
                    //TODO: should the Args.Count be done here or in the option? Does it concern the command? Consider calling option.Validate()
                    if (option.Args == null || option.Args.Count == 0)
                    {
                        throw new Exception($"{this.Name} requires the {option.Name} option.");
                    }
                }
            }
        }

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
