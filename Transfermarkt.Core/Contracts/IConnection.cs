using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core.Contracts
{
    public interface IConnection
    {
        IConnector Connector { get; set; }

        //IList<Competition> GetCompetitions(int season, Nationality? nationality = null);
        //IList<Club> GetClubs();
        //IList<Player> GetPlayers(int season, Nationality? nationality = null);

        Player ParsePlayer(string url);
        Club ParseSquad(string url);
        Competition ParseSquadsFromCompetition(string url);
    }
}
