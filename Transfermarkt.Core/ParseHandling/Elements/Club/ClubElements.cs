using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.Club
{
    public class Name : Element<StringValue>
    {
        public Name() : base("Name", "Name")
        {
            this.Value = new StringValue();
        }
    }

    public class Season : Element<IntValue>
    {
        public Season() : base("Y", "Season")
        {
            this.Value = new IntValue();
        }
    }

    public class ImgUrl : Element<StringValue>
    {
        public ImgUrl() : base("ImgUrl", "Img Url")
        {
            this.Value = new StringValue();
        }
    }

    public class Country : Element<NationalityValue>
    {
        public Country() : base("Country", "Country")
        {
            this.Value = new NationalityValue();
        }
    }

    public class CountryImg : Element<StringValue>
    {
        public CountryImg() : base("CountryImg", "Country Img")
        {
            this.Value = new StringValue();
        }
    }
}
