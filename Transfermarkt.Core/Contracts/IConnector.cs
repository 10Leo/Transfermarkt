using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Contracts
{
    public interface IConnector
    {
        void ConnectToPage(string url);
        DataTable GetCompetitionTable();
        DataTable GetTableByID(string id);
        DataTable GetTableByClass(string className);
    }
}
