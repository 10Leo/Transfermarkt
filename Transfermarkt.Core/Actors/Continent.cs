using Page.Parser.Contracts;
using System.Collections.Generic;

namespace Transfermarkt.Core.Actors
{
    public class Continent : Domain
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
