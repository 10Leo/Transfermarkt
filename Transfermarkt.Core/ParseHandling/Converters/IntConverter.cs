using Page.Parser.Contracts;

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
