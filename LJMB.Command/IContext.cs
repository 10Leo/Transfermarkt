using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJMB.Command
{
    public interface IContext
    {
        bool Exit { get; set; }

        void RegisterCommand(ICommand command);
        void Run();
    }
}
