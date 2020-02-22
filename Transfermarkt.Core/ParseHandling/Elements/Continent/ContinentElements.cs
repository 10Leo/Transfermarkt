using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.Continent
{
    public class Name : Element
    {
        public Name() : base("Name", "Name")
        {
        }
    }

    public class ContinentCode : Element
    {
        public ContinentCode() : base("Code", "Continent Code")
        {
        }
    }
}
