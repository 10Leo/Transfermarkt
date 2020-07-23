using Page.Scraper.Contracts;
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
