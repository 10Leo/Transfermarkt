using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.Continent
{
    class Name : Element
    {
        public Name() : base("Name", "Name")
        {
        }
    }

    class ContinentCode : Element
    {
        public ContinentCode() : base("Code", "Continent Code")
        {
        }
    }
}
