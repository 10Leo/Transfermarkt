using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core
{
    public interface ITransfermarktConnector
    {
        IList<Competition> GetCompetitions(int season, Nationality? nationality = null);
        IList<Club> GetClubs();
        IList<Player> GetPlayers(int season, Nationality? nationality = null);
    }
}
