using System.Collections.Generic;

namespace LJMB.Command
{
    public interface IOption
    {
        string Name { get; }
        ISet<string> AllowedAlias { get; }
        ISet<IArgument> Args { get; }

        void Parse(string toParse);
        void Reset();
    }
}
