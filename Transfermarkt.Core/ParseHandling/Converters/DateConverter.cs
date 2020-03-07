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
            return new DatetimeValue { Value = DateTime.ParseExact(stringToConvert, dateFormat, CultureInfo.InvariantCulture) };
        }
    }
}
