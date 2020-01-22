using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    class StringConverter : IConverter<string>
    {
        public string Convert(string stringToConvert)
        {
            return stringToConvert;
        }
    }
}
