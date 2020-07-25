using Page.Scraper.Contracts;
using System;
using System.Globalization;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    public class DateConverter : IConverter<DatetimeValue>
    {
        private readonly string dateFormat = "dd/MM/yyyy";
        
        public DatetimeValue Convert(string stringToConvert)
        {
            return new DatetimeValue { Value = DateTime.ParseExact(stringToConvert, dateFormat, CultureInfo.InvariantCulture) };
        }
    }
}
