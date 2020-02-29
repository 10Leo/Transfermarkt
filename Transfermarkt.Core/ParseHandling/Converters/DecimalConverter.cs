using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class DecimalConverter : IConverter<DecimalValue>
    {
        public DecimalValue Convert(string stringToConvert)
        {
            decimal? converted = null;
            try
            {
                converted = decimal.Parse(stringToConvert);
            }
            catch (Exception) { }
            return new DecimalValue { Value = converted };
        }
    }
}
