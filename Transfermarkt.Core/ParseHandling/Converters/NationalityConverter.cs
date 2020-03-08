using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts.Converter;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    public class NationalityConverter : INationalityConverter
    {
        public NationalityValue Convert(string stringToConvert)
        {
            return new NationalityValue { Value = ConvertersConfig.GetNationality(stringToConvert) };
        }
    }
}
