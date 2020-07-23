using System.Collections.Generic;

namespace Page.Parser.Contracts
{
    public interface IDomain
    {
        IList<IElement<IValue>> Elements { get; }

        IList<IDomain> Children { get; }

        IElement<IValue> SetElement(IElement<IValue> element);
    }
}
