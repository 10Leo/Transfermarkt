using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJMB.Command
{
    public interface IOption
    {
        string Name { get; set; }
        ISet<string> AllowedAlias { get; }
        ISet<IArgument> Args { get; set; }

        void Parse(string toParse);
    }
}
