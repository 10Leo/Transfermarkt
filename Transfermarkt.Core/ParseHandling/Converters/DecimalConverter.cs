using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class DecimalConverter : IConverter<DecimalValue>
    {
        private ILogger logger;

        public DecimalConverter(ILogger logger)
        {
            this.logger = logger;
        }

        public DecimalValue Convert(string stringToConvert)
        {
            return new DecimalValue { Value = decimal.Parse(stringToConvert) };
        }
    }
}
