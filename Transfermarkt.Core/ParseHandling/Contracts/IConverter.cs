namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IConverter<TValue>
    {
        TValue Convert(string stringToConvert);
    }
}
