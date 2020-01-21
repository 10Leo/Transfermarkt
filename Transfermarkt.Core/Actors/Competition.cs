using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Actors
{
    public class Competition : IDomain
    {
        public string Name { get; set; }
        public Nationality? Country { get; set; }
        public string CountryImg { get; set; }
        public int? Season { get; set; }
        public string ImgUrl { get; set; }
        public IList<Club> Clubs { get; set; }

        public Competition()
        {
            Clubs = new List<Club>();
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}] {2}\n{3}"
                , Name
                , Country
                , Season
                , string.Join(
                    "\n"
                    , Clubs.Select(
                        p => p.ToString()
                    )
                )
            );
        }
    }
}
