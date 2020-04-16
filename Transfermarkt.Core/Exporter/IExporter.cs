using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.Exporter
{
    public interface IExporter//<TDomain, TValue> where TDomain : IDomain<TValue> where TValue : IValue
    {
        void Extract(IDomain<IValue> domain);
    }
}
