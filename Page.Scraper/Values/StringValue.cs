using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page.Parser.Contracts
{
    public class StringValue : IValue
    {
        public Type Type => typeof(string);
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
}
