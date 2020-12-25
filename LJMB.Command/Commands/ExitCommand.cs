using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJMB.Command.Commands
{
    public class ExitCommand : Command
    {
        public const string KEY = "e";
        public const string NAME = "Exit";

        public ExitCommand(IProcessor context)
        {
            this.Name = NAME.ToLower();
            this.AllowedAlias.Add(KEY);
            this.AllowedAlias.Add(NAME.ToLower());
            this.Context = context;
            //this.Context.RegisterCommand(this);
        }

        public override void Parse(string completeCmdToParse) { }

        public override void Execute()
        {
            Context.Exit = true;
        }
    }
}
