using System;
using System.Globalization;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class DateConverter : IConverter<DateTime?>
    {
        private readonly string dateFormat = "dd/MM/yyyy";

        public DateTime? Convert(string stringToConvert)
        {
            DateTime? converted = null;
            try
            {
                converted = DateTime.ParseExact(stringToConvert, dateFormat, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
            return converted;
        }
    }
}
