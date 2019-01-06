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
using Transfermarkt.Core.Exporter;
using Transfermarkt.Exporter.JSONExporter;

namespace Transfermarkt.Console
{
    class Program
    {
        private static string BaseURL { get; } = ConfigurationManager.AppSettings["BaseURL"].ToString();
        private static IParser conn;
        private static IExporter exporter;

        static void Main(string[] args)
        {
            conn = new Parser(
                new HtmlAgilityPackConnector(),
                new PTNationalityConverter(),
                new PTPositionConverter(),
                new PTFootConverter()
            );

            exporter = new JsonExporter();
            
            System.Console.WriteLine("----------------------------------");
            //TestCompetition(conn);
            TestSquad(conn);
        }

        static void TestCompetition(IParser conn)
        {
            int season = 2018;
            string url = BaseURL + "/serie-a/startseite/wettbewerb/IT1/plus/?saison_id=" + season;

            try
            {
                Competition competition = conn.ParseSquadsFromCompetition(url);

                System.Console.WriteLine(competition);
                exporter.ExtractCompetition(competition);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.InnerException);
                System.Console.WriteLine(ex.Message);
                throw;
            }
        }

        static void TestSquad(IParser conn)
        {
            string url = BaseURL + "/fc-barcelona/kader/verein/131/saison_id/2018/plus/1";
            url = BaseURL + "/cd-nacional/kader/verein/982/saison_id/2018/plus/1";
            url = BaseURL + "/vitoria-sc/kader/verein/2420/saison_id/2018/plus/1";

            try
            {
                Club club = conn.ParseSquad(url);

                System.Console.WriteLine(club);
                exporter.ExtractClub(club);
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
