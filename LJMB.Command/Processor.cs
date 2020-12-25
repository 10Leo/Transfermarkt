using System;
using System.Collections.Generic;
using System.Linq;

namespace LJMB.Command
{
    public abstract class Processor : IProcessor
    {
        public IList<ICommand> Commands { get; }  = new List<ICommand>();

        public Func<IEnumerable<string>> GetCommands { get; set; }

        public bool Exit { get; set; } = false;

        public virtual void Run()
        {
            //while (!Exit)
            {
                try
                {
                    foreach (var inputCmd in GetCommands?.Invoke())
                    {
                        //var inputCmd = GetCmd?.Invoke();

                        if (!string.IsNullOrEmpty(inputCmd))
                        {
                            Command.ParseAndExecuteCommand(inputCmd, Commands);
                        }
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
    }
}
