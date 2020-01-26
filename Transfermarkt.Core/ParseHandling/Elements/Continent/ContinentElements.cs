using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.Continent
{
    class Name : Element
    {
        public Name() : base("Name", "Name")
        {
        }
    }

    class ContinentCode : Element
    {
        public ContinentCode() : base("Code", "Continent Code")
        {
        }
    }
}
