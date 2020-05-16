using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using Transfermarkt.Core;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Exporter;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Pages;
using Transfermarkt.Exporter.JSONExporter;
using Transfermarkt.Logging;

namespace Transfermarkt.Console
{
    class Program
    {
        private static string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);
        private static string PlusClubUrlFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.PlusClubUrlFormatV2);
        private static string CompetitionUrlFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.CompetitionUrlFormat);
        private static int MinimumLoggingLevel { get; } = ConfigManager.GetAppSetting<int>(Keys.Config.MinimumLoggingLevel);
        private static string LogPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.LogPath);

        private static readonly ILogger logger = LoggerFactory.GetLogger(LogPath, MinimumLoggingLevel);

        private static IExporter exporter;

        private static IDictionary<int, Type> pageTypes = new Dictionary<int, Type>();

        private static readonly string[] continentUrls = new string[]
        {
            "https://www.transfermarkt.pt/wettbewerbe/europa",
            "https://www.transfermarkt.pt/wettbewerbe/amerika",
            "https://www.transfermarkt.pt/wettbewerbe/asien",
            "https://www.transfermarkt.pt/wettbewerbe/afrika"
        };

        private static readonly IDictionary<int, (string displayName, string internalName)> continent = new Dictionary<int, (string, string)>
        {
            [1] = ("Europe", "europa"),
            [2] = ("America", "amerika"),
            [3] = ("Asia", "asien"),
            [4] = ("Africa", "afrika"),
        };

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
            pageTypes.Add(2, typeof(ContinentPage));
            pageTypes.Add(3, typeof(CompetitionPage));
            pageTypes.Add(4, typeof(ClubPage));
            exporter = new JsonExporter();

            System.Console.WriteLine("----------------------------------");

            try
            {
                // Continent
                System.Console.WriteLine("Escolha uma das seguintes opções:");

                System.Console.WriteLine(string.Format("0: Todas"));
                var vs = continent.Values.ToList();
                for (int i = 0; i < vs.Count; i++)
                {
                    System.Console.WriteLine(string.Format("{0}: {1}", continent.Keys.ElementAt(i), vs[i].displayName));
                }


                Command continentCmd = GetInput();

                //List<List<Link>> continentsCompetitionsUrls = Get(continentCmd, continentsCompetitionsUrls);

                var continentPages = new List<IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>>();
                var continentsCompetitionsUrls = new List<List<Link>>();
                foreach (var input in continentCmd.Options)
                {
                    string chosenContinent = $"{BaseURL}/wettbewerbe/{continent[input.Index1].internalName}";

                    var continentPage = (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>)Activator.CreateInstance(pageTypes[2], new HAPConnection(), logger);
                    continentPages.Add(continentPage);
                    List<Link> competitionsUrls = continentPage.Fetch(chosenContinent);
                    continentsCompetitionsUrls.Add(competitionsUrls);

                    if (continentCmd.CommandType == CommandType.P)
                    {
                        continentPage.Parse(chosenContinent);
                        exporter.Extract(continentPage.Domain);
                        return;
                    }
                }
                CheckIfExit(continentCmd);



                // Competitions
                PresentOptions(continentsCompetitionsUrls);

                Command competitionCmd = GetInput();

                List<List<Link>> clubsCompetitionsUrls = Execute(competitionCmd, pageTypes[3], continentsCompetitionsUrls);
                CheckIfExit(competitionCmd);



                // Clubs
                PresentOptions(clubsCompetitionsUrls);

                Command clubCmd = GetInput();
                clubCmd.CommandType = CommandType.P;

                List<List<Link>> clubUrls = Execute(clubCmd, pageTypes[4], clubsCompetitionsUrls);
                CheckIfExit(clubCmd);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error reading or interpreting chosen option.");
                System.Console.WriteLine(ex.Message);
                throw;
            }
        }

        private static Command GetInput()
        {
            string input = System.Console.ReadLine();
            return Util.ParseCommand(input);
        }

        private static void PresentOptions(List<List<Link>> urls)
        {
            System.Console.WriteLine("Escolha uma das seguintes opções:");
            System.Console.WriteLine(string.Format("0: Todas"));
            for (int i = 0; i < urls.Count; i++)
            {
                System.Console.WriteLine(string.Format("{0}", i + 1));
                for (int j = 0; j < urls[i].Count; j++)
                {
                    System.Console.WriteLine(string.Format("\t{0}.{1}: {2}", (i + 1), (j + 1), urls[i][j].Title));
                }
            }
        }

        private static List<List<Link>> Execute(Command cmd, Type type, List<List<Link>> urls)
        {
            var pages = new List<IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>>();
            var childUrlsCollection = new List<List<Link>>();
            foreach (var (Index1, Index2) in cmd.Options)
            {
                string choice = $"{urls[Index1 - 1][Index2 - 1].Url}";

                (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> Page, List<Link> Links) e = ExecuteAction(cmd, type, choice);
                pages.Add(e.Page);
                childUrlsCollection.Add(e.Links);
            }

            return childUrlsCollection;
        }

        private static (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> Page, List<Link> Links) ExecuteAction(Command cmd, Type type, string url)
        {
            var page = (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>)Activator.CreateInstance(type, new HAPConnection(), logger);
            List<Link> childUrls = null;

            switch (cmd.CommandType)
            {
                case CommandType.F:
                    childUrls = page.Fetch(url);
                    break;
                case CommandType.P:
                    childUrls = page.Fetch(url);
                    page.Parse(url);
                    exporter.Extract(page.Domain);
                    break;
                default:
                    break;
            }

            return (page, childUrls);
        }

        private static void CheckIfExit(Command cmd)
        {
            if (cmd.CommandType == CommandType.P)
            {
                Environment.Exit(0);
            }
        }
    }
}
