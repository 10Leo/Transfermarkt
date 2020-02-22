using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Converters
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
