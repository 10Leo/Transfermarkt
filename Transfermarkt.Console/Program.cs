using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core;
using Transfermarkt.Core.Connectors;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            IConnection conn = new TransfermarktConnection(new HtmlAgilityPackConnector());

            string url = "https://www.transfermarkt.pt/fc-barcelona/kader/verein/131/saison_id/2018/plus/1";
            conn.ParseSquad(url);
        }
    }
}
