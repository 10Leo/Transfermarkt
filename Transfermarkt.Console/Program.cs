using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static string BaseURL { get; } = ConfigurationManager.AppSettings["BaseURL"].ToString();
        private static IConnection conn;

        static void Main(string[] args)
        {
            conn = new TransfermarktConnection(
                new HtmlAgilityPackConnector(),
                new PTNationalityConverter(),
                new PTPositionConverter(),
                new PTFootConverter()
            );

            //TestSquad(conn);

            System.Console.WriteLine("----------------------------------");
            TestCompetition(conn);
        }

        static void TestCompetition(IConnection conn)
        {
            int season = 2018;
            string url = BaseURL + "/serie-a/startseite/wettbewerb/IT1/plus/?saison_id=" + season;

            try
            {
                Competition competition = conn.ParseSquadsFromCompetition(url);

                System.Console.WriteLine(competition);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.InnerException);
                System.Console.WriteLine(ex.Message);
                throw;
            }
        }

        static void TestSquad(IConnection conn)
        {
            string url = BaseURL + "/fc-barcelona/kader/verein/131/saison_id/2018/plus/1";
            url = BaseURL + "/cd-nacional/kader/verein/982/saison_id/2018/plus/1";
            url = BaseURL + "/vitoria-sc/kader/verein/2420/saison_id/2018/plus/1";

            try
            {
                Club club = conn.ParseSquad(url);

                System.Console.WriteLine(club);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.InnerException);
                System.Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
