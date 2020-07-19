using Page.Parser.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class IntConverter : IConverter<IntValue>
    {
        public IntValue Convert(string stringToConvert)
        {
            return new IntValue { Value = int.Parse(stringToConvert) };
        }
    }
}
