using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core
{
    public class TransfermarktConnection : IConnection
    {
        public IConnector Connector { get; set; }

        public IList<Competition> GetCompetitions(int season, Nationality? nationality = null)
        {
            throw new NotImplementedException();
        }

        public IList<Club> GetClubs()
        {
            throw new NotImplementedException();
        }

        public IList<Player> GetPlayers(int season, Nationality? nationality = null)
        {
            IList<Player> players = new List<Player>();

            return players;
        }
    }
}
