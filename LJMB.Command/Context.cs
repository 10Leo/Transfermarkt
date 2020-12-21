using System;
using System.Collections.Generic;
using System.Linq;

namespace LJMB.Command
{
    public abstract class Context : IContext
    {
        protected readonly IList<ICommand> Commands = new List<ICommand>();

        public bool Exit { get; set; } = false;

        public virtual void Run()
        {
            while (!Exit)
            {
                try
                {
                    var inputCmd = GetInput();

                    if (!string.IsNullOrEmpty(inputCmd))
                    {
                        Command.ParseAndExecuteCommand(inputCmd, Commands);
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ErrorMsg.ERROR_MSG_INTERPRET);
                    System.Console.WriteLine(ex.Message);
                }
            }
        }

        public void RegisterCommand(ICommand command)
        {
            if (Commands.Any(c => c.Name == command.Name))
            {
                throw new Exception("Command already registered.");
            }
            Commands.Add(command);
        }

        private static string GetInput()
        {
            System.Console.Write("> ");
            string input = System.Console.ReadLine();
            return input;
        }
    }
}
