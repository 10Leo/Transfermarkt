using Transfermarkt.Core.ParseHandling.Contracts.Converter;

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
