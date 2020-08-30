using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJMB.Command.Commands
{
    public class ExitCommand : Command
    {
        public ExitCommand(IContext context)
        {
            this.Name = "exit";
            this.AllowedAlias.Add("e");
            this.AllowedAlias.Add("exit");
            this.Context = context;
            this.Context.RegisterCommand(this);
        }

        public override void Parse(string completeCmdToParse) { }

        public override void Execute()
        {
            Context.Exit = true;
        }
    }
}
