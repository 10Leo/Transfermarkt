using System.Collections.Generic;
using System.Linq;

namespace Transfermarkt.Core.Actors
{
    public class Club
    {
        public string Name { get; set; }
        public Nationality? Country { get; set; }
        public string CountryImg { get; set; }
        public int Season { get; set; }
        public string ImgUrl { get; set; }
        public IList<Player> Squad { get; set; }

        public Club()
        {
            Squad = new List<Player>();
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}] {2}\n{3}"
                , Name
                , Country
                , Season
                , string.Join(
                    "\n"
                    , Squad.Select(
                        p => p.ToString()
                    )
                )
            );
        }
    }
}
