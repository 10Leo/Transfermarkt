using Page.Parser.Contracts;
using System;
using System.Globalization;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class DateConverter : IConverter<DatetimeValue>
    {
        private readonly string dateFormat = "dd/MM/yyyy";
        
        public DatetimeValue Convert(string stringToConvert)
        {
            return new DatetimeValue { Value = DateTime.ParseExact(stringToConvert, dateFormat, CultureInfo.InvariantCulture) };
        }
    }
}
