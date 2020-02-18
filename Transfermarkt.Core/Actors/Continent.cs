﻿using System.Collections.Generic;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.Actors
{
    public class Continent : Domain
    {
        public Continent()
        {
            Elements = new List<IElement<object>>
            {
                new Transfermarkt.Core.ParseHandling.Elements.Continent.Name(),
                new Transfermarkt.Core.ParseHandling.Elements.Continent.ContinentCode()
            };
        }
    }
}
