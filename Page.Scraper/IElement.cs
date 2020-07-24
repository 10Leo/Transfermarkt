namespace Page.Scraper.Contracts
{
    public interface IElement<out TValue, out TConverter> where TValue : IValue where TConverter : IConverter<TValue>
    {
        string Name { get; }
        string InternalName { get; }
        TValue Value { get; }
        TConverter Converter { get; }
    }
}