using System;

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
