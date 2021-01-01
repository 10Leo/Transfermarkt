using LJMB.Command.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJMB.Command.Test
{
    public class Util {
        public const string ExitCmd = "e";
        public const string ListCmd = "l";
    }

    public class MockProcessor: Processor
    {
        public MockProcessor()
        {
        }
    }
}
