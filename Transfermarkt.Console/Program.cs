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

        private static IDictionary<string, (int id, string internalName)> clubs = new Dictionary<string, (int, string)>
        {
            ["Barcelona"] = (131, "fc-barcelona"),
            ["Nacional"] = (982, "cd-nacional"),
            ["V. Guimarães"] = (2420, "vitoria-sc"),
        };

        private static IDictionary<string, (string internalName, string d1, string d2)> competitions = new Dictionary<string, (string internalName, string d1, string d2)>
        {
            ["ITA"] = ("serie-a", "IT1", "")
        };

        static void Main(string[] args)
        {
            conn = new Parser(
                new HtmlAgilityPackConnector(),
                new ConvertersCollection(
                    new PTNationalityConverter(),
                    new PTPositionConverter(),
                    new PTFootConverter()
                )
            );

            exporter = new JsonExporter();

            System.Console.WriteLine("----------------------------------");
            //TestCompetition(conn, "ITA, 2018");
            TestSquad(conn, "Barcelona", 2011);
        }

        static void TestCompetition(IParser conn, string countryISO3, int season = 2018)
        {
            string detailsPageUrl = $"/{competitions[countryISO3].internalName}/startseite/wettbewerb/{competitions[countryISO3].d1}/plus/?saison_id={season}";
            
            string url = BaseURL + detailsPageUrl;

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

        static void TestSquad(IParser conn, string name, int season = 2018)
        {
            string detailsPageUrl = $"/{clubs[name].internalName}/kader/verein/{clubs[name].id}/plus/1/galerie/0?saison_id={season}";

            string url = BaseURL + detailsPageUrl;

            //url = BaseURL + $"/cd-nacional/kader/verein/982/saison_id/{season}/plus/1";
            //url = BaseURL + $"/vitoria-sc/kader/verein/2420/saison_id/{season}/plus/1";

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
