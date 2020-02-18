using System.Collections.Generic;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.Actors
{
    public class Competition : Domain
    {
        public Competition()
        {
            Elements = new List<IElement<object>>
            {
                //new Country(),
                //new Name(),
                //new Season(),
                //new ImgUrl(),
                //new CountryImg()
            };
        }
    }
}
