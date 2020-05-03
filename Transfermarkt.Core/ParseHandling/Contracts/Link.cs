using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public class Link
    {
        public string Title { get; set; }
        public string Url { get; set; }

        public override string ToString()
        {
            return $"{(Title ?? Url)}";
        }
    }
}
