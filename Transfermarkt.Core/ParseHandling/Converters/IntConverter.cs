using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class IntConverter : IConverter<IntValue>
    {
        private ILogger logger;

        public IntConverter(ILogger logger)
        {
            this.logger = logger;
        }

        public IntValue Convert(string stringToConvert)
        {
            return new IntValue { Value = int.Parse(stringToConvert) };
        }
    }
}
