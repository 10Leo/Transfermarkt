using Page.Parser.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class DecimalConverter : IConverter<DecimalValue>
    {
        public DecimalValue Convert(string stringToConvert)
        {
            return new DecimalValue { Value = decimal.Parse(stringToConvert) };
        }
    }
}
