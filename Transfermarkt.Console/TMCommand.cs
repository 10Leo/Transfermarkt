using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LJMB.Command
{
    public abstract class TMCommand : Command
    {
    }

    public enum OptionName
    {
        /// <summary>
        /// Year.
        /// </summary>
        Y,
        /// <summary>
        /// Page(s) indexes to fetch/parse.
        /// </summary>
        I
    }
}
