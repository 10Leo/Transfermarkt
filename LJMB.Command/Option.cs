using System;
using System.Collections.Generic;

namespace LJMB.Command
{
    public abstract class Option : IOption
    {
        public string Name { get; set; }

        public ISet<string> AllowedAlias { get; }

        public ISet<IArgument> Args { get; set; } = new HashSet<IArgument>();

        public abstract void Parse(string value);

        public void Reset()
        {
            Args.Clear();
        }
    }
}
