using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LJMB.Command
{
    public class CommandProcessor
    {
        public static ICommand ParseCommand(string inputCmd, IList<ICommand> commands)
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
    }
}
