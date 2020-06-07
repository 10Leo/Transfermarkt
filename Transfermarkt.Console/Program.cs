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

        private const decimal ContinentFactor = 1000000000;
        private const decimal CompetitionFactor = 1000000;
        private const decimal ClubFactor = 1000;

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

        private static readonly IDictionary<string, Link> continent = new Dictionary<string, Link>
        {
            ["1"] = new Link { Title = "Europe", Url = $"{BaseURL}/wettbewerbe/europa" },
            ["2"] = new Link { Title = "America", Url = $"{BaseURL}/wettbewerbe/amerika" },
            ["3"] = new Link { Title = "Asia", Url = $"{BaseURL}/wettbewerbe/asien" },
            ["4"] = new Link { Title = "Africa", Url = $"{BaseURL}/wettbewerbe/afrika" }
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
                PresentOptions(continent);
                Command continentCmd = GetInput();
                IDictionary<string, Link> competitionsLinks = Execute(continentCmd, pageTypes[2], continent);
                CheckIfExit(continentCmd);



                // Competitions
                PresentOptions(competitionsLinks);
                Command competitionCmd = GetInput();
                IDictionary<string, Link> clubsCompetitionsUrls = Execute(competitionCmd, pageTypes[3], competitionsLinks);
                CheckIfExit(competitionCmd);



                // Clubs
                PresentOptions(clubsCompetitionsUrls);
                Command clubCmd = GetInput();
                clubCmd.CommandType = CommandType.P;
                IDictionary<string, Link> clubUrls = Execute(clubCmd, pageTypes[4], clubsCompetitionsUrls);
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
            return CommandUtil.ParseCommand(input);
        }

        private static void PresentOptions(IDictionary<string, Link> urls)
        {
            System.Console.WriteLine("Escolha uma das seguintes opções:");
            System.Console.WriteLine(string.Format("0: Todas"));
            for (int i = 0; i < urls.Count; i++)
            {
                var v = urls.Keys.ElementAt(i);
                System.Console.WriteLine(string.Format("\t{0}: {1}", v, (!string.IsNullOrEmpty(urls[v].Title) ? urls[v].Title : urls[v].Url)));

                //Match splitArguments = Regex.Match(v.ToString(), @"(?<Continent>[0-9]?[0-9]?[0-9])(?<League>[0-9][0-9][0-9])(?<Club>[0-9][0-9][0-9])(?<Player>[0-9][0-9][0-9])$");
                //var continent = int.Parse(splitArguments.Groups["Continent"].Value);
                //var competition = int.Parse(splitArguments.Groups["League"].Value);
                //var club = int.Parse(splitArguments.Groups["Club"].Value);

                //var ss = new List<int>();
                //if (continent > 0)
                //{
                //    ss.Add(continent);
                //}
                //if (competition > 0)
                //{
                //    ss.Add(competition);
                //}
                //if (club > 0)
                //{
                //    ss.Add(club);
                //}

                //System.Console.WriteLine(string.Format("\t{0}: {1}", string.Join(".", ss), (!string.IsNullOrEmpty(urls[v].Title) ? urls[v].Title : urls[v].Url)));
            }
        }

        private static IDictionary<string, Link> Execute(Command cmd, Type type, IDictionary<string, Link> urls)
        {
            var pages = new List<IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>>();
            var childUrlsCollection = new Dictionary<string, Link>();

            IParameterValue fir = cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.O).Val;

            if (fir is Index1ParameterValue)
            {
                List<int> indexes = ((Index1ParameterValue)cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.O).Val).Indexes;

                foreach (int index in indexes)
                {
                    string choice = $"{urls[index.ToString()].Url}";

                    (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> Page, List<Link> Links) e = ExecuteAction(cmd, type, choice);
                    pages.Add(e.Page);

                    for (int i = 0; i < e.Links.Count; i++)
                    {
                        childUrlsCollection.Add($"{index + "." + (i + 1)}", e.Links[i]);
                    }
                }
            }
            else if (fir is Index2ParameterValue)
            {
                List<(int Index1, int Index2)> indexes = ((Index2ParameterValue)cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.O).Val).Indexes;

                foreach (var (Index1, Index2) in indexes)
                {
                    string choice = $"{urls[Index1 + "." + Index2].Url}";

                    (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> Page, List<Link> Links) e = ExecuteAction(cmd, type, choice);
                    pages.Add(e.Page);

                    for (int i = 0; i < e.Links.Count; i++)
                    {
                        childUrlsCollection.Add($"{Index2 + "." + (i + 1)}", e.Links[i]);
                    }
                }
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
