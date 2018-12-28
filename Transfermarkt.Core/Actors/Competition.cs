using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Actors
{
    public class Competition
    {
        public int Season { get; set; }
        public IList<Club> Clubs { get; set; }

        public Competition()
        {
            Clubs = new List<Club>();
        }
    }
}
