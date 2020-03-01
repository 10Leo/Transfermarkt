using System;
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
            decimal? converted = null;
            try
            {
                converted = decimal.Parse(stringToConvert);
            }
            catch (Exception ex) {
                logger.LogException(LogLevel.Error, $"The string {stringToConvert} wasn't found on the config file.", ex);
            }

            return new DecimalValue { Value = converted };
        }
    }
}
