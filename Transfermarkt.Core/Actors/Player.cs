using System.Collections.Generic;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.Actors
{
    public class Player : Domain<object>
    {
        public Player()
        {
            Elements = new List<IElement<object>>
            {
                new Transfermarkt.Core.ParseHandling.Elements.Player.Name(),
                //new Transfermarkt.Core.ParseHandling.Elements.Player.ShortName(),
                new Transfermarkt.Core.ParseHandling.Elements.Player.BirthDate(),
                //new Transfermarkt.Core.ParseHandling.Elements.Player.Nationality(),
                //new Transfermarkt.Core.ParseHandling.Elements.Player.Height(),

                //new Transfermarkt.Core.ParseHandling.Elements.Player.PreferredFoot(),
                //new Transfermarkt.Core.ParseHandling.Elements.Player.Position(),

                new Transfermarkt.Core.ParseHandling.Elements.Player.ShirtNumber(),
                //new Transfermarkt.Core.ParseHandling.Elements.Player.Captain(),
                //new Transfermarkt.Core.ParseHandling.Elements.Player.ClubArrivalDate(),
                //new Transfermarkt.Core.ParseHandling.Elements.Player.ContractExpirationDate(),

                //new Transfermarkt.Core.ParseHandling.Elements.Player.MarketValue(),

                ////new Transfermarkt.Core.ParseHandling.Elements.Player.ImgUrl(),
                //new Transfermarkt.Core.ParseHandling.Elements.Player.ProfileUrl(),
            };
        }
    }
}
