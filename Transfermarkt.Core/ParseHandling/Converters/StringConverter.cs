using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class StringConverter : IConverter<StringValue>
    {
        private ILogger logger;

        public StringConverter(ILogger logger)
        {
            this.logger = logger;
        }

        public StringValue Convert(string stringToConvert)
        {
            return new StringValue { Value = stringToConvert };
        }
    }
}
