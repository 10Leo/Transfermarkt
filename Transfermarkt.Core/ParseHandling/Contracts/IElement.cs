namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IElement<out TValue>
    {
        string Name { get; }
        string InternalName { get; }
        TValue Value { get; }
    }
}