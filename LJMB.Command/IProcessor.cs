using System;
using System.Collections.Generic;

namespace LJMB.Command
{
    public interface IProcessor
    {
        //TODO: create a unit test project for this feature
        IList<ICommand> Commands { get; }

        Func<IEnumerable<string>> GetCommands { get; set; }

        bool Exit { get; set; }

        void RegisterCommand(ICommand command);

        ICommand Find(string inputCmd, IList<ICommand> commands);
        void Execute(ICommand sentCmd, string inputCmd, IList<ICommand> commands);
        void Run();
    }
}
