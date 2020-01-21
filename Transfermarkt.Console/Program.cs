using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core;
using Transfermarkt.Core.Actors;
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

        private static IExporter exporter;

        private static readonly IDictionary<string, (int id, string internalName)> clubs = new Dictionary<string, (int, string)>
        {
            ["Barcelona"] = (131, "fc-barcelona"),
            ["Nacional"] = (982, "cd-nacional"),
            ["V. Guimarães"] = (2420, "vitoria-sc"),
        };

        private static IDictionary<Nationality, (string internalName, string d1, string d2)> competitions = new Dictionary<Nationality, (string internalName, string d1, string d2)>
        {
            [Nationality.ITA] = ("serie-a", "IT1", "")
        };

        private static int currentSeason = (DateTime.Today.Year < 8) ? DateTime.Today.Year - 1 : DateTime.Today.Year;

        static void Main(string[] args)
        {
            exporter = new JsonExporter();

            System.Console.WriteLine("----------------------------------");

            int option = 0;
            if (args.Length > 0)
            {
                try
                {
                    option = int.Parse(args[0]);
                }
                catch (Exception)
                {
                    //log
                }
            }
            switch (option)
            {
                case 0:
                    TestCompetitions();
                    break;
                case 1:
                    TestCompetition(Nationality.ITA, currentSeason);
                    break;
                case 2:
                    TestSquad("Barcelona", 2011);
                    break;
                default: break;
            }
        }

        static void TestCompetitions(int season = 2018)
        {
            //string url = BaseURL + detailsPageUrl;
            //try
            //{
            //    Season s = conn.ParseCompetitions(url, season);

            //    System.Console.WriteLine(competition);
            //    exporter.ExtractCompetitions(competition);
            //}
            //catch (Exception ex)
            //{
            //    System.Console.WriteLine(ex.InnerException);
            //    System.Console.WriteLine(ex.Message);
            //    throw;
            //}
        }

        static void TestCompetition(Nationality nationality, int season = 2018)
        {
            string detailsPageUrl = CompetitionUrlFormat;
            detailsPageUrl = detailsPageUrl.Replace("{COMPETITION_NAME}", competitions[nationality].internalName);
            detailsPageUrl = detailsPageUrl.Replace("{DIVISION}", competitions[nationality].d1);
            detailsPageUrl = detailsPageUrl.Replace("{SEASON}", season.ToString());

            string url = BaseURL + detailsPageUrl;
            try
            {
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.InnerException);
                System.Console.WriteLine(ex.Message);
                throw;
            }
        }

        static void TestSquad(string name, int season = 2018)
        {
            string detailsPageUrl = PlusClubUrlFormat;
            detailsPageUrl = detailsPageUrl.Replace("{CLUB_STRING}", clubs[name].internalName);
            detailsPageUrl = detailsPageUrl.Replace("{CLUB_ID}", clubs[name].id.ToString());
            detailsPageUrl = detailsPageUrl.Replace("{SEASON}", season.ToString());

            string url = BaseURL + detailsPageUrl;
            try
            {
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
