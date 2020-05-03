using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

            IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> page = null;
            exporter = new JsonExporter();

            System.Console.WriteLine("----------------------------------");
            System.Console.WriteLine("Escolha uma das seguintes opções:");

            var vs = continent.Values.ToList();
            for (int i = 0; i < vs.Count; i++)
            {
                System.Console.WriteLine(string.Format("{0}: {1}", continent.Keys.ElementAt(i), vs[i].displayName));
            }

            int val = RequestNumber();
            string opt = continent[val].internalName;

            System.Console.WriteLine("Parse: 0; Peek: 1");
            int typ = RequestNumber();

            page = (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>)Activator.CreateInstance(pageTypes[2], new HAPConnection(), logger);

            if (typ == 0)
            {
                page.Fetch($"{opt}");
                page.Parse($"{BaseURL}/wettbewerbe/{opt}");
                return;
            }


            List<string> urls = page.Fetch($"{BaseURL}/wettbewerbe/{opt}");

            System.Console.WriteLine("Escolha uma das seguintes opções:");
            System.Console.WriteLine(string.Format("0: Todas"));

            for (int i = 0; i < urls.Count; i++)
            {
                System.Console.WriteLine(string.Format("{0}: {1}", (i + 1), urls[i]));
            }

            try
            {
                List<int> opts = RequestNumbers();

                System.Console.WriteLine("Parse: 0; Peek: 1");
                typ = RequestNumber();

                if (typ == 0)
                {
                    var parsedLeagues = new List<IDomain<IValue>>();
                    foreach (var item in opts)
                    {
                        opt = urls[item - 1];

                        page = (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>)Activator.CreateInstance(pageTypes[3], new HAPConnection(), logger);

                        page.Fetch($"{opt}");
                        IDomain<IValue> league = page.Parse($"{opt}");
                        parsedLeagues.Add(league);
                    }
                    return;
                }

                List<List<string>> leaguesClubsUrls = new List<List<string>>();
                foreach (var item in opts)
                {
                    opt = urls[item - 1];

                    page = (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>)Activator.CreateInstance(pageTypes[3], new HAPConnection(), logger);

                    List<string> league = page.Fetch($"{opt}");
                    leaguesClubsUrls.Add(league);
                }


                System.Console.WriteLine("The chosen options will be parsed.");
                System.Console.WriteLine(string.Format("0: Todas"));
                int k = 0;
                foreach (List<string> leagueClubs in leaguesClubsUrls)//league X clubs
                {
                    for (int i = 0; i < leagueClubs.Count; i++)
                    {
                        System.Console.WriteLine(string.Format("{0}{1}: {2}", k, (i + 1), leagueClubs[i]));
                    }
                    k++;
                }

                List<int[]> opts30 = RequestNumbers2();

                //System.Console.WriteLine("Parse: 0; Peek: 1");
                //typ = RequestNumber();

                List<IDomain<IValue>> parsedClubs = new List<IDomain<IValue>>();

                {
                    foreach (int[] item in opts30)
                    {
                        opt = leaguesClubsUrls[item[0]][item[1] - 1];

                        page = (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>)Activator.CreateInstance(pageTypes[4], new HAPConnection(), logger);

                        page.Fetch($"{opt}");
                        IDomain<IValue> club = page.Parse($"{opt}");
                        parsedClubs.Add(club);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error reading or interpreting chosen option.");
                System.Console.WriteLine(ex.Message);
                throw;
            }
        }


        private static int RequestUrls()
        {
            int operation = 0;
            return operation;
        }

        private static List<int> RequestNumbers()
        {
            List<int> numbers = new List<int>();

            try
            {
                string strNumbers = System.Console.ReadLine();
                string[] splitedStrNumbers = strNumbers.Split(' ');

                foreach (var strNumber in splitedStrNumbers)
                {
                    numbers.Add(int.Parse(strNumber));
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error reading or interpreting supplied numbers.");
                System.Console.WriteLine(ex.Message);
                throw;
            }

            return numbers;
        }

        private static List<int[]> RequestNumbers2()
        {
            List<int[]> numbers = new List<int[]>();

            try
            {
                string strNumbers = System.Console.ReadLine();
                string[] splitedStrNumbers = strNumbers.Split(' ');

                foreach (var strNumber in splitedStrNumbers)
                {
                    string[] splitedStrNumbers2 = strNumber.Split('.');

                    int[] ns = new int[2];
                    for (int i = 0; i < splitedStrNumbers2.Length; i++)
                    {
                        ns[i] = int.Parse(splitedStrNumbers2[i]);
                    }
                    numbers.Add(ns);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error reading or interpreting supplied numbers.");
                System.Console.WriteLine(ex.Message);
                throw;
            }

            return numbers;
        }

        private static int RequestNumber()
        {
            int operation = 0;

            try
            {
                string reqOperation = System.Console.ReadLine();
                operation = int.Parse(reqOperation);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error reading or interpreting supplied operation.");
                System.Console.WriteLine(ex.Message);
                throw;
            }

            return operation;
        }
    }
}
