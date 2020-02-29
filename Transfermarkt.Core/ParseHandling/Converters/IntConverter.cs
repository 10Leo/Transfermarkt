using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class IntConverter : IConverter<IntValue>
    {
        public IntValue Convert(string stringToConvert)
        {
            int? converted = null;
            try
            {
                converted = int.Parse(stringToConvert);
            }
            catch (Exception) { }
            return new IntValue { Value = converted };
        }
    }
}
