using Page.Scraper.Contracts;
using Transfermarkt.Core.ParseHandling.Converters;

namespace Transfermarkt.Core.ParseHandling.Elements.Club
{
    public class Name : Element<StringValue, StringConverter>
    {
        public Name() : this(null) { }

        public Name(string value) : base("Name", "Name", value) { }
    }

    public class Season : Element<IntValue, IntConverter>
    {
        public Season() : this(null) { }

        public Season(string value) : base("Y", "Season", value) { }
    }

    public class ImgUrl : Element<StringValue, StringConverter>
    {
        public ImgUrl() : this(null) { }

        public ImgUrl(string value) : base("ImgUrl", "Img Url", value) { }
    }

    public class Country : Element<NationalityValue, NationalityConverter>
    {
        public Country() : this(null) { }

        public Country(string value) : base("Country", "Country", value) { }
    }

    public class CountryImg : Element<StringValue, StringConverter>
    {
        public CountryImg() : this(null) { }

        public CountryImg(string value) : base("CountryImg", "Country Img", value) { }
    }
}
