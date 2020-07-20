using Transfermarkt.Core.ParseHandling.Contracts.Converter;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    public class PositionConverter : IPositionConverter
    {
        public PositionValue Convert(string stringToConvert)
        {
            return new PositionValue { Value = ConvertersConfig.GetPosition(stringToConvert) };
        }
    }
}
