using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Console
{
    public interface ICommand
    {
        string Name { get; set; }

        bool CanParse(string cmdToParse);
        void Parse(string completeCmdToParse);
        void Validate();
        void Execute();
    }
}
