using Page.Scraper.Contracts;
using Transfermarkt.Core.ParseHandling.Converters;

namespace Transfermarkt.Core.ParseHandling.Elements.Continent
{
    public class Name : Element<StringValue, StringConverter>
    {
        public Name() : this(null) { }

        public Name(string value) : base("Name", "Name", value) { }
    }

    public class ContinentCode : Element<ContinentCodeValue, ContinentCodeConverter>
    {
        public ContinentCode() : this(null) { }

        public ContinentCode(string value) : base("Code", "Continent Code", value) { }
    }
}
