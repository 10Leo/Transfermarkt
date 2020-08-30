using System.Collections.Generic;

namespace LJMB.Command
{
    public interface ICommand
    {
        IOption this[string option] { get; }

        string Name { get; set; }

        IContext Context { get; set; }
        ISet<IOption> Options { get; set; }

        bool CanParse(string toParse);
        void Parse(string toParse);
        void Validate();
        void Execute();
        void Reset();
    }
}
