using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Actors
{
    public class Player : Person
    {
        public int Number { get; set; }
        public int Height { get; set; }
        public int PreferedFoot { get; set; }
        public DateTime ClubArrivalDate { get; set; }
        public DateTime ContractExpirationDate { get; set; }
        public int MarketValue { get; set; }
    }
}
