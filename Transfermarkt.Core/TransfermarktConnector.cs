using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mshtml;
using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core
{
    public class TransfermarktConnector : ITransfermarktConnector
    {
        

        public IList<Club> GetClubs()
        {
            throw new NotImplementedException();
        }

        public IList<Competition> GetCompetitions(int season, Nationality? nationality = null)
        {
            throw new NotImplementedException();
        }

        public IList<Player> GetPlayers(int season, Nationality? nationality = null)
        {
            IList<Player> players = new List<Player>();

            HTMLDocument doc;

            return players;
        }
    }
}
