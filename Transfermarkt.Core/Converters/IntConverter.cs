using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Converters
{
    class IntConverter : IConverter<int?>
    {
        public int? Convert(string stringToConvert)
        {
            int? converted = null;
            try
            {
                converted = int.Parse(stringToConvert);
            }
            catch (Exception) { }
            return converted;
        }
    }
}
