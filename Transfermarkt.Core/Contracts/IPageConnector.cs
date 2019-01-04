using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Contracts
{
    public interface IPageConnector
    {
        void ConnectToPage(string url);
        (string country, string countryImg, string Name, int Season, string ImgUrl) GetCompetitionData();
        (string country, string countryImg, string Name, int Season, string ImgUrl) GetClubData();
        DataTable GetCompetitionClubsTable();
        DataTable GetClubSquadTable();
    }
}
