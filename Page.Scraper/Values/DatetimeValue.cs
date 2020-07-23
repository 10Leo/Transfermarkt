using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page.Scraper.Contracts
{
    public class DatetimeValue : IValue
    {
        private readonly string dateFormat = "dd-MM-yyyy";

        public Type Type => typeof(DateTime?);
        public DateTime? Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Value?.ToString(dateFormat, CultureInfo.InvariantCulture));
        }
    }
}
