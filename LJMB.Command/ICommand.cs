using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJMB.Command
{
    public interface ICommand
    {
        string Name { get; set; }

        ISet<IOption> Options { get; set; }

        bool CanParse(string toParse);
        void Parse(string toParse);
        void Validate();
        void Execute();
    }
}
