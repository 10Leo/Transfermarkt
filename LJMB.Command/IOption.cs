using System.Collections.Generic;

namespace LJMB.Command
{
    public interface IOption
    {
        string Name { get; }
        bool Active { get; set; }

        ISet<string> AllowedAlias { get; }
        ISet<IArgument> Args { get; }

        void Parse(string toParse);
        void Reset();
    }
}
