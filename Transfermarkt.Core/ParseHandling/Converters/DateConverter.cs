using System;
using System.Globalization;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class DateConverter : IConverter<DatetimeValue>
    {
        private readonly string dateFormat = "dd/MM/yyyy";
        private ILogger logger;

        public DateConverter(ILogger logger)
        {
            this.logger = logger;
        }

        public DatetimeValue Convert(string stringToConvert)
        {
            DateTime? converted = null;
            //TODO: remove try catch inside IConverters? exceptions are being handled in the class that calls this function
            try
            {
                converted = DateTime.ParseExact(stringToConvert, dateFormat, CultureInfo.InvariantCulture);
            }
            catch (Exception ex) {
                logger.LogException(LogLevel.Error, $"The string {stringToConvert} wasn't found on the config file.", ex);
            }

            return new DatetimeValue { Value = converted };
        }
    }
}
