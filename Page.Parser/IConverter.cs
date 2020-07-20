namespace Page.Parser.Contracts
{
    public interface IConverter<TValue> where TValue : IValue
    {
        TValue Convert(string stringToConvert);
    }
}
