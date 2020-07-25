using System.Collections.Generic;

namespace Page.Scraper.Contracts
{
    public interface IDomain
    {
        IList<IElement<IValue, IConverter<IValue>>> Elements { get; }

        IList<IDomain> Children { get; }

        IElement<IValue, IConverter<IValue>> SetElement(IElement<IValue, IConverter<IValue>> element);
    }
}
