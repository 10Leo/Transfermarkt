using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Actors
{
    public class Person
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Nationality? Nationality { get; set; }
        public string ImgUrl { get; set; }
    }
}
