namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IElement
    {
        string Name { get; }
        string InternalName { get; }
        IValue Value { get; set; }
    }
}