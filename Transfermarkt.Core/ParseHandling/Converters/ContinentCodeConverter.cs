using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class ContinentCodeConverter : IConverter<ContinentCodeValue>
    {
        public ContinentCodeValue Convert(string stringToConvert)
        {
            return new ContinentCodeValue { Value = ConvertersConfig.GetContinent(stringToConvert) };
        }
    }
}
