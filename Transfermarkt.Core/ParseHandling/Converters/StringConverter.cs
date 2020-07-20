using Page.Parser.Contracts;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class StringConverter : IConverter<StringValue>
    {
        public StringValue Convert(string stringToConvert)
        {
            return new StringValue { Value = stringToConvert };
        }
    }
}
