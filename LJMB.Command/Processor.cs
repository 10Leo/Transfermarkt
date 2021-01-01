using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LJMB.Command
{
    public abstract class Processor : IProcessor
    {
        public IList<ICommand> Commands { get; } = new List<ICommand>();

        public Func<IEnumerable<string>> GetCommands { get; set; }

        public bool Exit { get; set; } = false;

        public virtual void Run()
        {
            //while (!Exit)
            foreach (var inputCmd in GetCommands?.Invoke())
            {
                try
                {
                    //var inputCmd = GetCmd?.Invoke();

                    if (!string.IsNullOrEmpty(inputCmd))
                    {
                        ICommand cmd = Find(inputCmd, Commands);
                        Execute(cmd, inputCmd, Commands);
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine();
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

        public virtual ICommand Find(string inputCmd, IList<ICommand> commands)
        {
            if (string.IsNullOrEmpty(inputCmd = inputCmd?.Trim()))
            {
                throw new Exception(ErrorMsg.ERROR_MSG_CMD);
            }

            var cmdGroup = "CMD";
            var m = Regex.Matches(inputCmd, $@"^(?<{cmdGroup}>\w)\s*");

            if (m == null || m.Count == 0)
            {
                throw new Exception(ErrorMsg.ERROR_MSG_CMD);
            }

            var cm = m[0].Groups[cmdGroup];
            if (string.IsNullOrEmpty(cm.Value) || string.IsNullOrWhiteSpace(cm.Value))
            {
                throw new Exception(ErrorMsg.ERROR_MSG_CMD);
            }


            ICommand sentCmd = null;
            foreach (ICommand c in commands)
            {
                if (c.CanParse(cm.Value.Trim().ToLowerInvariant()))
                {
                    sentCmd = c;
                    break;
                }
            }

            return sentCmd;
        }

        public virtual void Execute(ICommand sentCmd, string inputCmd, IList<ICommand> commands)
        {
            if (sentCmd == null)
            {
                throw new Exception(ErrorMsg.ERROR_MSG_CMD_NOT_FOUND);
            }

            try
            {
                sentCmd.Parse(inputCmd);
                sentCmd.Execute();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sentCmd.Reset();
            }
        }
    }
}
