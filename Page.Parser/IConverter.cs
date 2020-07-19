namespace Page.Parser.Contracts
{
    public interface IConverter<TValue>
    {
        TValue Convert(string stringToConvert);
    }
}
