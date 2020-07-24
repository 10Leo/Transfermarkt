using Page.Scraper.Contracts;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    public class IntConverter : IConverter<IntValue>
    {
        public IntValue Convert(string stringToConvert)
        {
            return new IntValue { Value = int.Parse(stringToConvert) };
        }
    }
}
