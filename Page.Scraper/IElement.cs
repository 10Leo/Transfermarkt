namespace Page.Scraper.Contracts
{
    public interface IElement<out TValue> where TValue : IValue
    {
        string Name { get; }
        string InternalName { get; }
        TValue Value { get; }
    }
}