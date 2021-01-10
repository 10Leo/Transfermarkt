using System.Collections.Generic;

namespace LJMB.Command
{
    public interface ICommand
    {
        IOption this[string option] { get; }

        string Name { get; }
        string Usage { get; }
        ISet<string> AllowedAlias { get; }

        IProcessor Context { get; }
        ISet<IOption> Options { get; }

        bool CanParse(string toParse);
        void Parse(string toParse);
        void Validate();
        void Execute();
        void Reset();
    }
}
