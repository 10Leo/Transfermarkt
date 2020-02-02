using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.Competition
{
    class Name : Element
    {
        public Name() : base("Name", "Name")
        {
        }
    }

    class Season : Element
    {
        public Season() : base("Y", "Season")
        {
        }
    }

    class ImgUrl : Element
    {
        public ImgUrl() : base("ImgUrl", "Img Url")
        {
        }
    }

    class Country : Element
    {
        public Country() : base("Country", "Country")
        {
        }
    }

    class CountryImg : Element
    {
        public CountryImg() : base("CountryImg", "Country Img")
        {
        }
    }
}
