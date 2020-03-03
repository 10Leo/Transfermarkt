using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts.Converter;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    public class PositionConverter : IPositionConverter
    {
        private ILogger logger;

        public PositionConverter() : this(null) { }

        public PositionConverter(ILogger logger)
        {
            this.logger = logger;
        }

        public PositionValue Convert(string stringToConvert)
        {
            return new PositionValue { Value = ConvertersConfig.GetPosition(stringToConvert) };
        }
    }
}
