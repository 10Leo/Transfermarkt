using System.Collections.Generic;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.Actors
{
    public class Continent : Domain<IValue>
    {
        public Continent()
        {
            Elements = new List<IElement<IValue>>
            {
                new Transfermarkt.Core.ParseHandling.Elements.Continent.Name(),
                new Transfermarkt.Core.ParseHandling.Elements.Continent.ContinentCode()
            };
        }
    }
}
