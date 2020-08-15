using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJMB.Command.Commands
{
    public class ExitCommand : Command
    {
        public IContext Context { get; set; }
        
        public ExitCommand(IContext context)
        {
            this.Name = "exit";
            this.Context = context;
            this.Context.RegisterCommand(this);
        }

        public override bool CanParse(string cmdToParse)
        {
            return cmdToParse == "e";
        }

        public override void Parse(string completeCmdToParse)
        {
        }

        public override void Validate()
        {
        }

        public override void Execute()
        {
            Context.Exit = true;
        }
    }
}
