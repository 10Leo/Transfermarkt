using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.Continent
{
    public class Name : Element<string>
    {
        public Name() : base("Name", "Name")
        {
        }
    }

    public class ContinentCode : Element<string>
    {
        public ContinentCode() : base("Code", "Continent Code")
        {
        }
    }
}
