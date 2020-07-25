namespace Page.Scraper.Contracts
{
    public abstract class Element<TValue, TConverter> : IElement<TValue, TConverter> where TConverter : IConverter<TValue>, new() where TValue : IValue, new()
    {
        public string InternalName { get; }
        public string Name { get; }

        public TConverter Converter { get; set; }

        public TValue Value { get; set; }

        public Element(string internalName, string name, string value = null)
        {
            InternalName = internalName;
            Name = name;
            Converter = new TConverter();
            if (Converter != null)
            {
                Value = value == null ? new TValue() : Converter.Convert(value);
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}: {1}]", InternalName, Value);
        }
    }
}
