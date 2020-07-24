using Page.Scraper.Contracts;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    public class DecimalConverter : IConverter<DecimalValue>
    {
        public DecimalValue Convert(string stringToConvert)
        {
            return new DecimalValue { Value = decimal.Parse(stringToConvert) };
        }
    }
}
