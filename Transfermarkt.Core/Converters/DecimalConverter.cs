using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Converters
{
    class DecimalConverter : IConverter<decimal?>
    {
        public decimal? Convert(string stringToConvert)
        {
            decimal? converted = null;
            try
            {
                converted = decimal.Parse(stringToConvert);
            }
            catch (Exception) { }
            return converted;
        }
    }
}
