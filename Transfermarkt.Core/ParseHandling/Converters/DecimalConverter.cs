using Page.Parser.Contracts;

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
