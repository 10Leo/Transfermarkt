using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Converters
{
    public class DateConverter : IConverter<DateTime?>
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
