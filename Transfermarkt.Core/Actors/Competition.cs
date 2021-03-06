﻿using System.Collections.Generic;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.Actors
{
    public class Competition : Domain<IValue>
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
