using System.Collections.Generic;

namespace LJMB.Command
{
    public abstract class Option : IOption
    {
        public string Name { get; protected set; }
        public bool Active { get; set; }

        //TODO: add prop that describes usage
        public ISet<string> AllowedAlias { get; protected set; }

        public ISet<IArgument> Args { get; protected set; } = new HashSet<IArgument>();

        public abstract void Parse(string toParse);

        public void Reset()
        {
            Active = false;
            Args.Clear();
        }
    }
}
