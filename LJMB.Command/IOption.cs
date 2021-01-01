using System.Collections.Generic;

namespace LJMB.Command
{
    public interface IOption
    {
        string Name { get; }
        bool Active { get; set; }
        bool Required { get; set; }
        string Usage { get; set; }

        ISet<string> AllowedAlias { get; }
        ISet<IArgument> Args { get; }

        void Validate();
        void Parse(string toParse);
        void Reset();
    }
}
