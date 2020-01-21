using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Actors
{
    public class Continent
    {
        public string Name { get; set; }
        public ContinentCode? ContinentCode { get; set; }
        public IList<Competition> Competitions { get; set; }

        public Continent()
        {
            Competitions = new List<Competition>();
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]\n{2}"
                , Name
                , ContinentCode
                , string.Join(
                    "\n"
                    , Competitions.Select(
                        p => p.ToString()
                    )
                )
            );
        }
    }
}
