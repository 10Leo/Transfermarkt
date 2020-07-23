namespace Page.Scraper.Contracts
{
    public interface IConverter<TValue> where TValue : IValue
    {
        TValue Convert(string stringToConvert);
    }
}
