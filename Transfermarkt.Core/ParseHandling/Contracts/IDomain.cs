using System.Collections.Generic;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IDomain
    {
        IList<IElement> Elements { get; }

        IList<IDomain> Children { get; }

        IElement SetElement(IElement element);
    }
}
