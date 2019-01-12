using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core.Exporter
{
    public interface IExporter
    {
        void ExtractCompetition(Competition competition);
        void ExtractClub(Club club);
    }
}
