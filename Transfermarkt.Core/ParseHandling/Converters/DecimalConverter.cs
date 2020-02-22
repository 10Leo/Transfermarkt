using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Converters
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
