using System;
using System.Collections.Generic;
using System.Globalization;
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
        public Foot? PreferredFoot { get; set; }
        public Position? Position { get; set; }
        public string Captain { get; set; }
        public DateTime? ClubArrivalDate { get; set; }
        public DateTime? ContractExpirationDate { get; set; }
        public decimal MarketValue { get; set; }

        public override string ToString()
        {
            var cultureInfo = CultureInfo.GetCultureInfo("pt-PT");
            return string.Format("{0} ({1}) {2} {3} {4}cm {5} {6} [{7}] {8} {9}-{10} [{11}] {12} {13}"
                , Name
                , ShortName
                , Nationality
                , BirthDate?.ToString("yyyy.MM.dd")
                , Height
                , PreferredFoot
                , Position
                , Number
                , Captain
                , ClubArrivalDate?.ToString("yyyy.MM.dd")
                , ContractExpirationDate?.ToString("yyyy.MM.dd")
                , string.Format(cultureInfo, "{0:C0}", MarketValue)
                , ProfileUrl
                , ImgUrl
            );
        }
    }
}
