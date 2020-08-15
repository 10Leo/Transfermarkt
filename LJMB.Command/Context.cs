using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LJMB.Command
{
    public abstract class Context : IContext
    {
        public bool Exit { get; set; } = false;

        protected readonly IList<ICommand> Commands = new List<ICommand>();

        //public Context()
        //{
        //    //Commands.Add(new ExitCommand(this));
        //}

        public virtual void Run()
        {
            while (!Exit)
            {
                try
                {
                    var inputCmd = GetInput();

                    if (!string.IsNullOrEmpty(inputCmd))
                    {
                        ParseAndExecuteCommand(inputCmd);
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

        private void ParseAndExecuteCommand(string inputCmd)
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
            if (cm == null || string.IsNullOrEmpty(cm.Value) || string.IsNullOrWhiteSpace(cm.Value))
            {
                throw new Exception(ErrorMsg.ERROR_MSG_CMD);
            }


            ICommand sentCmd = null;
            foreach (ICommand c in Commands)
            {
                if (c.CanParse(cm.Value.Trim().ToLowerInvariant()))
                {
                    sentCmd = c;
                    break;
                }
            }

            if (sentCmd == null)
            {
                throw new Exception(ErrorMsg.ERROR_MSG_CMD_NOT_FOUND);
            }

            sentCmd.Parse(inputCmd);
            sentCmd.Execute();
        }
    }
}
