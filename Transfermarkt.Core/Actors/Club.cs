﻿using System.Collections.Generic;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Club;

namespace Transfermarkt.Core.Actors
{
    public class Club : Domain
    {
        public Club()
        {
            Elements = new List<IElement<object>>
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
