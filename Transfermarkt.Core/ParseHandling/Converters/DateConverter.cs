using System;
using System.Globalization;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class DateConverter : IConverter<DatetimeValue>
    {
        private readonly string dateFormat = "dd/MM/yyyy";

        public DatetimeValue Convert(string stringToConvert)
        {
            DateTime? converted = null;
            //TODO: remove try catch inside IConverters? exceptions are being handled in the class that calls this function
            try
            {
                converted = DateTime.ParseExact(stringToConvert, dateFormat, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            return new DatetimeValue { Value = converted };
        }
    }
}
