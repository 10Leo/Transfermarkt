using System;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class IntConverter : IConverter<IntValue>
    {
        private ILogger logger;

        public IntConverter(ILogger logger)
        {
            this.logger = logger;
        }

        public IntValue Convert(string stringToConvert)
        {
            int? converted = null;
            try
            {
                converted = int.Parse(stringToConvert);
            }
            catch (Exception ex)
            {
                logger.LogException(LogLevel.Error, $"The string {stringToConvert} wasn't found on the config file.", ex);
            }

            return new IntValue { Value = converted };
        }
    }
}
