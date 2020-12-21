using System;
using System.Collections.Generic;

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
