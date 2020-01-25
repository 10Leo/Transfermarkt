using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    abstract class Element : IElement
    {
        public string InternalName { get; }
        public string Name { get; }
        public dynamic Value { get; set; }

        public Element(string internalName, string name)
        {
            InternalName = internalName;
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("[{0}: {1}]", InternalName, Value);
        }
    }
}
