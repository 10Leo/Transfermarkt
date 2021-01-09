using Page.Scraper.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.ParseHandling.Converters;

namespace Transfermarkt.Core.ParseHandling.Elements.Continents
{
    public class Name : Element<StringValue, StringConverter>
    {
        public Name() : this(null) { }

        public Name(string value) : base("Name", "Name", value) { }
    }
}
