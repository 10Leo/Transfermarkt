using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Transfermarkt.Core.Elements.Player;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.Actors
{
    class Player : IDomain
    {
        public IList<IElement> Elements { get; set; }
        public IList<IDomain> Children { get; set; } = null;

        public Player()
        {
            Elements = new List<IElement>();
            Elements.Add(new MarketValue());
            Elements.Add(new Height());

            Children = new List<IDomain>();
        }

        public IElement SetElement(IElement element)
        {
            if (element == null)
            {
                return null;
            }
            var elementType = element.GetType();

            foreach (var e in Elements)
            {
                if (e.GetType() == elementType)
                {
                    e.Value = element.Value;
                    return e;
                }
            }

            return null;
        }

        public override string ToString()
        {
            return string.Format("{0}\n{1}"
                , string.Join(
                    ", "
                    , Children.Select(
                        p => p.ToString()
                    )
                )
                , string.Join(
                    ", "
                    , Elements.Select(
                        p => p.ToString()
                    )
                )
            );
        }
    }
}
