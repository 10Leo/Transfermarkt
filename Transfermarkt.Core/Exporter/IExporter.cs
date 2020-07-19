using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.Exporter
{
    public interface IExporter
    {
        void Extract(IDomain domain);
    }
}
