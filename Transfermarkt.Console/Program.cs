using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Connectors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.Converters;

namespace Transfermarkt.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            IConnection conn = new TransfermarktConnection(
                new HtmlAgilityPackConnector(),
                new PTNationalityConverter(),
                new PTPositionConverter(),
                new PTFootConverter()
            );

            string url = "https://www.transfermarkt.pt/fc-barcelona/kader/verein/131/saison_id/2018/plus/1";
            url = "https://www.transfermarkt.pt/cd-nacional/kader/verein/982/saison_id/2018/plus/1";
            url = "https://www.transfermarkt.pt/vitoria-sc/kader/verein/2420/saison_id/2018/plus/1";

            Club club = conn.ParseSquad(url);

            System.Console.WriteLine("----------------------------------");
            System.Console.WriteLine(club);
        }
    }
}
