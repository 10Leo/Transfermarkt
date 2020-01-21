using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Converters
{
    class StringConverter : IConverter<string>
    {
        public string Convert(string stringToConvert)
        {
            return stringToConvert;
        }
    }
}
