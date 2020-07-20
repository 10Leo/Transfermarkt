using Page.Parser.Contracts;
using System.Collections.Generic;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.Actors
{
    public class Competition : Domain
    {
        public Competition()
        {
            Elements = new List<IElement<IValue>>
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
