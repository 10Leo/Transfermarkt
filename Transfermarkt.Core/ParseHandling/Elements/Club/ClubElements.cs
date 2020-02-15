using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.Club
{
    public class Name : Element<string>
    {
        public Name() : base("Name", "Name")
        {
        }
    }

    public class Season : Element<int>
    {
        public Season() : base("Y", "Season")
        {
        }
    }

    public class ImgUrl : Element<string>
    {
        public ImgUrl() : base("ImgUrl", "Img Url")
        {
        }
    }

    public class Country : Element<Actors.Nationality>
    {
        public Country() : base("Country", "Country")
        {
        }
    }

    public class CountryImg : Element<string>
    {
        public CountryImg() : base("CountryImg", "Country Img")
        {
        }
    }
}
