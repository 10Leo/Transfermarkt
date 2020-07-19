namespace Page.Parser.Contracts
{
    public interface IElement<out TValue> where TValue : IValue
    {
        string Name { get; }
        string InternalName { get; }
        TValue Value { get; }
    }
}