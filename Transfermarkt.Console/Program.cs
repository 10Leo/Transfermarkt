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
        private static string PlusClubUrlFormat { get; } = ConfigurationManager.AppSettings["PlusClubUrlFormatV2"].ToString();
        private static string CompetitionUrlFormat { get; } = ConfigurationManager.AppSettings["CompetitionUrlFormat"].ToString();

        private static IParser conn;
        private static IExporter exporter;

        private static IDictionary<string, (int id, string internalName)> clubs = new Dictionary<string, (int, string)>
        {
            ["Barcelona"] = (131, "fc-barcelona"),
            ["Nacional"] = (982, "cd-nacional"),
            ["V. Guimarães"] = (2420, "vitoria-sc"),
        };

        private static IDictionary<Nationality, (string internalName, string d1, string d2)> competitions = new Dictionary<Nationality, (string internalName, string d1, string d2)>
        {
            [Nationality.ITA] = ("serie-a", "IT1", "")
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


            //TestCompetition(conn, Nationality.ITA, 2018);

            System.Console.WriteLine("----------------------------------");
            TestSquad(conn, "Barcelona", 2011);
        }

        static void TestCompetition(IParser conn, Nationality nationality, int season = 2018)
        {
            string detailsPageUrl = CompetitionUrlFormat;
            detailsPageUrl = detailsPageUrl.Replace("{COMPETITION_NAME}", competitions[nationality].internalName);
            detailsPageUrl = detailsPageUrl.Replace("{DIVISION}", competitions[nationality].d1);
            detailsPageUrl = detailsPageUrl.Replace("{SEASON}", season.ToString());

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
            string detailsPageUrl = PlusClubUrlFormat;
            detailsPageUrl = detailsPageUrl.Replace("{CLUB_STRING}", clubs[name].internalName);
            detailsPageUrl = detailsPageUrl.Replace("{CLUB_ID}", clubs[name].id.ToString());
            detailsPageUrl = detailsPageUrl.Replace("{SEASON}", season.ToString());

            string url = BaseURL + detailsPageUrl;
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
