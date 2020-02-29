using System.Collections.Generic;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IDomain<TValue> where TValue : IValue
    {
        IList<IElement<TValue>> Elements { get; }

        IList<IDomain<TValue>> Children { get; }

        IElement<TValue> SetElement(IElement<TValue> element);
    }
}
