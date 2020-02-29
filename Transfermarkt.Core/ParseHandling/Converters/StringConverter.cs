using Transfermarkt.Core.ParseHandling.Contracts;

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
