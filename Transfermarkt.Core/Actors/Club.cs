using System.Collections.Generic;
using System.Linq;

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
            return string.Format("{0}\n{1}"
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
