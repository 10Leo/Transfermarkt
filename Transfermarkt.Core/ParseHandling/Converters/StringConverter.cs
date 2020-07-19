using Page.Parser.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Logging;

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
