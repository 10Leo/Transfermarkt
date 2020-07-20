using Page.Parser.Contracts;

namespace Transfermarkt.Core.Exporter
{
    public interface IExporter
    {
        void Extract(IDomain domain);
    }
}
