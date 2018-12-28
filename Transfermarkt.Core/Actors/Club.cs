using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Actors
{
    public class Club
    {
        public int Season { get; set; }
        public IList<Player> Squad { get; set; }

        public Club()
        {
            Squad = new List<Player>();
        }

        public override string ToString()
        {
            return string.Format("{0} {1}"
                , Season
                , string.Join(
                    "\n"
                    , Squad.Select(
                        p => string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13}"
                            , p.ProfileUrl
                            , p.Number
                            , p.Name
                            , p.ShortName
                            , p.ImgUrl
                            , p.Position
                            , p.Captain
                            , p.Nationality
                            , p.BirthDate
                            , p.Height
                            , p.PreferredFoot
                            , p.ClubArrivalDate
                            , p.ContractExpirationDate
                            , p.MarketValue
                        )
                    )
                )
            );
        }
    }
}
