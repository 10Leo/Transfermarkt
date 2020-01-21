namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IConverter<T>
    {
        T Convert(string stringToConvert);
    }
}
