using Page.Scraper.Contracts;

namespace Page.Scraper.Exporter
{
    public interface IExporter
    {
        void Extract(IDomain domain);
    }
}
