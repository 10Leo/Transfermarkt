using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.Continent
{
    public class Name : Element<StringValue>
    {
        public Name() : base("Name", "Name")
        {
            this.Value = new StringValue();
        }
    }

    public class ContinentCode : Element<ContinentCodeValue>
    {
        public ContinentCode() : base("Code", "Continent Code")
        {
            this.Value = new ContinentCodeValue();
        }
    }
}
