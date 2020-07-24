namespace Page.Scraper.Contracts
{
    public interface IConverter<out TValue> where TValue : IValue
    {
        TValue Convert(string stringToConvert);
    }
}
