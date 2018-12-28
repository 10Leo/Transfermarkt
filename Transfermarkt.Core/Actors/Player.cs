using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Actors
{
    public class Player : Person
    {
        public string ProfileUrl { get; set; }
        public int Number { get; set; }
        public int Height { get; set; }
        public string PreferredFoot { get; set; }
        public string Position { get; set; }
        public string Captain { get; set; }
        public DateTime ClubArrivalDate { get; set; }
        public DateTime ContractExpirationDate { get; set; }
        public int MarketValue { get; set; }
    }
}
