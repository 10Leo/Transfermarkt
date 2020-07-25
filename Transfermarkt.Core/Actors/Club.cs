using Page.Scraper.Contracts;
using System.Collections.Generic;
using Transfermarkt.Core.ParseHandling.Elements.Club;

namespace Transfermarkt.Core.Actors
{
    public class Club : Domain
    {
        public Club()
        {
            Elements = new List<IElement<IValue, IConverter<IValue>>>
            {
                new Country(),
                new Name(),
                new Season(),
                new ImgUrl(),
                new CountryImg()
            };
        }
    }
}
