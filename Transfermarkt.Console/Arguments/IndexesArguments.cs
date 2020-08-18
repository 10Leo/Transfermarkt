using LJMB.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Console.Arguments
{
    public class Index1Argument : IArgument
    {
        public int Index1 { get; set; }
    }

    public class Index2Argument : IArgument
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
    }

    public class Index3Argument : IArgument
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public int Index3 { get; set; }
    }
}
