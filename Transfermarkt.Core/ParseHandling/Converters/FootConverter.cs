using Transfermarkt.Core.ParseHandling.Contracts.Converter;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    public class FootConverter : IFootConverter
    {
        public FootValue Convert(string stringToConvert)
        {
            return new FootValue { Value = ConvertersConfig.GetFoot(stringToConvert) };
        }
    }
}
