using System.Collections.Generic;

namespace LJMB.Command
{
    public abstract class Option : IOption
    {
        public string Name { get; protected set; }

        public ISet<string> AllowedAlias { get; protected set; }

        public ISet<IArgument> Args { get; protected set; } = new HashSet<IArgument>();

        public abstract void Parse(string value);

        public void Reset()
        {
            Args.Clear();
        }
    }
}
