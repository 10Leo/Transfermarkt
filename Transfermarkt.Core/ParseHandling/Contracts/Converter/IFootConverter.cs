using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core.ParseHandling.Contracts.Converter
{
    // TODO: These sub IConverters need to be replaced outside of the contracts, as they are domain specific (these ones belong to transfermarkt, they aren't need in a different domain like the fifa one for the site scraper)
    public interface IFootConverter : IConverter<FootValue>
    {
    }
}
