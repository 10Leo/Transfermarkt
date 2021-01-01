using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJMB.Command.Commands
{
    public class ListCommand : Command
    {
        public const string KEY = "l";
        public const string NAME = "List";

        public ListCommand(IProcessor context)
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
            foreach (ICommand cmd in Context.Commands)
            {
                Console.WriteLine($"{cmd.Usage}");
            }
        }
    }
}
