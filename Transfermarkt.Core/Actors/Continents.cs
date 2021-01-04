using Page.Scraper.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Actors
{
    public class Continents : Domain
    {
        public Continents()
        {
            Elements = new List<IElement<IValue, IConverter<IValue>>>
            {
                new Transfermarkt.Core.ParseHandling.Elements.Continents.Name()
            };
        }
    }
}
