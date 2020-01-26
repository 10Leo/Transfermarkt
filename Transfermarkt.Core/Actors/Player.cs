using System.Collections.Generic;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.Actors
{
    class Player : IDomain
    {
        public IList<IElement> Elements { get; set; }
        public IList<IDomain> Children { get; set; } = null;

        public Player()
        {
            Elements = new List<IElement>
            {
                new Transfermarkt.Core.ParseHandling.Elements.Player.Name(),
                new Transfermarkt.Core.ParseHandling.Elements.Player.ShortName(),
                new Transfermarkt.Core.ParseHandling.Elements.Player.BirthDate(),
                new Transfermarkt.Core.ParseHandling.Elements.Player.Nationality(),
                new Transfermarkt.Core.ParseHandling.Elements.Player.Height(),

                new Transfermarkt.Core.ParseHandling.Elements.Player.PreferredFoot(),
                new Transfermarkt.Core.ParseHandling.Elements.Player.Position(),

                new Transfermarkt.Core.ParseHandling.Elements.Player.ShirtNumber(),
                new Transfermarkt.Core.ParseHandling.Elements.Player.Captain(),
                new Transfermarkt.Core.ParseHandling.Elements.Player.ClubArrivalDate(),
                new Transfermarkt.Core.ParseHandling.Elements.Player.ContractExpirationDate(),

                new Transfermarkt.Core.ParseHandling.Elements.Player.MarketValue(),

                new Transfermarkt.Core.ParseHandling.Elements.Player.ImgUrl(),
                new Transfermarkt.Core.ParseHandling.Elements.Player.ProfileUrl(),
            };

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
