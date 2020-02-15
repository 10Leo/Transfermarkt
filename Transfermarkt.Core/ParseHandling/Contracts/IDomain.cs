using System.Collections.Generic;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IDomain<out TValue>
    {
        IEnumerable<IElement<TValue>> Elements { get; }

        IEnumerable<IDomain<TValue>> Children { get; }

        //IElement<TValue> SetElement(IElement<TValue> element);
    }
}
