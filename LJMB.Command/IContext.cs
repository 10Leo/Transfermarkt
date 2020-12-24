using System;
using System.Collections.Generic;

namespace LJMB.Command
{
    public interface IContext
    {
        IList<ICommand> Commands { get; }

        Func<IEnumerable<string>> GetCommands { get; set; }

        bool Exit { get; set; }

        void RegisterCommand(ICommand command);
        void Run();
    }
}
