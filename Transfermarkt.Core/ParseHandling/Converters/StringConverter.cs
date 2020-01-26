using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class StringConverter : IConverter<object>
    {
        public object Convert(string stringToConvert)
        {
            return stringToConvert;
        }
    }
}
