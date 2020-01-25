using System.Collections.Generic;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IDomain
    {
        IList<IElement> Elements { get; set; }

        IList<IDomain> Children { get; set; }

        IElement SetElement(IElement element);
    }
}
