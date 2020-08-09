using HtmlAgilityPack;
using Page.Scraper.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using Transfermarkt.Core;
using Transfermarkt.Core.Actors;
using Page.Scraper.Exporter;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Pages;
using Page.Scraper.Exporter.JSONExporter;
using LJMB.Logging;
using LJMB.Common;

namespace Transfermarkt.Console
{
    class Program
    {
        private static string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);
        private static string PlusClubUrlFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.PlusClubUrlFormatV2);
        private static string CompetitionUrlFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.CompetitionUrlFormat);
        private static int MinimumLoggingLevel { get; } = ConfigManager.GetAppSetting<int>(Keys.Config.MinimumLoggingLevel);
        private static string LogPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.LogPath);
        public static string OutputFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.OutputFolderPath);
        public static string Level1FolderFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.Level1FolderFormat);
        public static string ContinentFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ContinentFileNameFormat);
        public static string CompetitionFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.CompetitionFileNameFormat);
        public static string ClubFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ClubFileNameFormat);

        private const decimal ContinentFactor = 1000000000;
        private const decimal CompetitionFactor = 1000000;
        private const decimal ClubFactor = 1000;

        private static readonly ILogger logger = LoggerFactory.GetLogger((LogLevel)MinimumLoggingLevel);

        private static IExporter exporter;

        private static int currentSeason = (DateTime.Today.Month < 8) ? DateTime.Today.Year - 1 : DateTime.Today.Year;
        private static string lastSelectedSeason = currentSeason.ToString();

        private static IDictionary<string, Type> pageTypes = new Dictionary<string, Type>();

        private static IDictionary<string, (Link L, ContinentPage P)> cont = new Dictionary<string, (Link, ContinentPage)>
        {
            [$"1"] = (new Link { Title = "Europe", Url = $"{BaseURL}/wettbewerbe/europa" }, null),
            [$"2"] = (new Link { Title = "America", Url = $"{BaseURL}/wettbewerbe/amerika" }, null),
            [$"3"] = (new Link { Title = "Asia", Url = $"{BaseURL}/wettbewerbe/asien" }, null),
            [$"4"] = (new Link { Title = "Africa", Url = $"{BaseURL}/wettbewerbe/afrika" }, null)
        };

        private static readonly IDictionary<string, (Link L, ContinentPage P)> continent = new Dictionary<string, (Link, ContinentPage)>();

        private static (Link L, ContinentPage P) choice;

        static void Main(string[] args)
        {
            pageTypes.Add("Continent", typeof(ContinentPage));
            pageTypes.Add("Competition", typeof(CompetitionPage));
            pageTypes.Add("Club", typeof(ClubPage));
            exporter = new JsonExporter(OutputFolderPath, Level1FolderFormat);

            System.Console.WriteLine("Transfermarkt Web Scrapper\n");


            for (int i = 0; i < cont.Count; i++)
            {
                System.Console.WriteLine($"{(i + 1)}: {cont.ElementAt(i).Value}");
            }

            bool exit = false;
            while (!exit)
            {
                try
                {
                    Command cmd = GetInput();

                    exit = CheckIfExit(cmd);

                    if (!exit)
                    {
                        // TODO: Create a service project (of Web Api type e.g.) to proccess these calls.
                        Proccess(cmd);
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Error reading or interpreting chosen option.");
                    System.Console.WriteLine(ex.Message);
                }
            }
        }

        private static Command GetInput()
        {
            System.Console.Write("> ");
            string input = System.Console.ReadLine();
            return CommandUtil.ParseCommand(input);
        }

        private static void Proccess(Command cmd)
        {
            IndexesParameterValue i = cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.I).Val as IndexesParameterValue;

            // Check if a year was passed by the user as an argument. If not get the last passed one, or the current one, if one was not passed yet. 
            var yy = cmd[ParameterName.Y];
            if (yy == null)
            {
                var y = new StringParameterValue
                {
                    Value = lastSelectedSeason
                };
                cmd.Parameters.Add((ParameterName.Y, y));
            }

            lastSelectedSeason = ((StringParameterValue)cmd[ParameterName.Y]).Value;

            foreach (IIndex ind in i.Indexes)
            {
                int i1 = 0;
                int i2 = 0;
                int i3 = 0;

                if (ind is Index1ParameterValue)
                {
                    i1 = (ind as Index1ParameterValue).Index1;
                }
                else if (ind is Index2ParameterValue)
                {
                    i1 = (ind as Index2ParameterValue).Index1;
                    i2 = (ind as Index2ParameterValue).Index2;
                }
                else if (ind is Index3ParameterValue)
                {
                    i1 = (ind as Index3ParameterValue).Index1;
                    i2 = (ind as Index3ParameterValue).Index2;
                    i3 = (ind as Index3ParameterValue).Index3;
                }

                bool proceed = ContinentsP(cmd, i1);
                //if (proceed && i1 != 0)
                //{
                //    proceed = ContinentP(cmd, $"{i1.ToString()}", ind is Index1ParameterValue);
                //}
                //if (proceed && i2 != 0)
                //{
                //    proceed = CompetitionP(cmd, $"{i1.ToString()}.{i2.ToString()}", i2, i3, ind is Index2ParameterValue);
                //}
                //if (proceed && i3 != 0)
                //{
                //    //proceed = ClubP(cmd, $"{i1.ToString()}.{i2.ToString()}.{i3.ToString()}", ind is Index3ParameterValue);
                //}

                if (proceed && i1 != 0)
                    Comm(cmd, i1 == 0 ? (int?)null : i1, i2 == 0 ? (int?)null : i2, i3 == 0 ? (int?)null : i3);
            }
        }

        private static bool ContinentsP(Command cmd, int i1)
        {
            var year = ((StringParameterValue)cmd[ParameterName.Y]).Value;

            var k = $"{year}.{i1}";

            if (!cont.ContainsKey(i1.ToString()))
            {
                return false;
            }

            if (!continent.ContainsKey(k))
            {
                continent.Add(k, (cont[i1.ToString()].L, null));
            }

            return true;
        }

        private static bool Comm(Command cmd, int? i1, int? i2, int? i3)
        {
            var year = int.Parse(((StringParameterValue)cmd[ParameterName.Y]).Value);

            var k = $"{year}.{i1}";

            if (!continent.ContainsKey(k))
            {
                return false;
            }
            (Link L, ContinentPage P) choice = continent[k];

            bool isFinal = !i2.HasValue && !i3.HasValue;

            if (choice.P == null || !choice.P.Connection.IsConnected)
            {
                choice.P = (ContinentPage)Activator.CreateInstance(pageTypes["Continent"], new HAPConnection(), logger, year);
                //var c = Activator.CreateInstance<ContinentPage>();
                //c.Connection = new HAPConnection();
                continent[k] = choice;

                choice.P.Connect(choice.L.Url);

                // do a fetch or a parse according to conditions.
                choice.P.Parse(parseChildren: cmd.Action == Action.P && isFinal);
            }

            var continentCompetitionsSection = (ChildsSection<HtmlNode, CompetitionPage>)choice.P["Continent - Competitions Section"];
            if (continentCompetitionsSection != null)
            {
                System.Console.WriteLine();
                PresentOptions(continentCompetitionsSection.Children, $"{i1}", 1);
            }

            if (cmd.Action == Action.P && isFinal)
            {
                exporter.Extract(choice.P.Domain, ContinentFileNameFormat);
            }

            if (isFinal || !i2.HasValue)
            {
                return true;
            }


            isFinal = !i3.HasValue;

            Link chosenCompetitionLink = continentCompetitionsSection.Children[i2.Value - 1];
            continentCompetitionsSection.Parse(new[] { chosenCompetitionLink }, parseChildren: cmd.Action == Action.P && isFinal);

            IPage<IDomain, HtmlNode> competitionPage = continentCompetitionsSection[new Dictionary<string, string> { { "Title", chosenCompetitionLink.Title } }];
            var clubsSection = (ChildsSection<HtmlNode, ClubPage>)competitionPage["Competition - Clubs Section"];
            if (clubsSection != null)
            {
                System.Console.WriteLine();
                PresentOptions(clubsSection.Children, $"{i1}.{i2}", 2);
            }

            if (cmd.Action == Action.P && isFinal)
            {
                exporter.Extract(competitionPage.Domain, CompetitionFileNameFormat);
            }

            if (isFinal || !i3.HasValue)
            {
                return true;
            }


            isFinal = true;

            Link chosenClubLink = clubsSection.Children[i3.Value - 1];
            clubsSection.Parse(new[] { chosenClubLink }, true);

            IPage<IDomain, HtmlNode> clubPage = clubsSection[new Dictionary<string, string> { { "Title", chosenClubLink.Title } }];
            var playersSection = (ChildsSamePageSection<Player, HtmlNode>)clubPage["Club - Players Section"];

            if (cmd.Action == Action.P && isFinal)
            {
                exporter.Extract(clubPage.Domain, ClubFileNameFormat);
            }

            return true;
        }

        //private static void PresentOptions(ChildsSection<HtmlNode, CompetitionPage> section, string key, int level)
        //{
        //    if (section == null)
        //    {
        //        return;
        //    }
        //    PresentOptions(section.Children, key, level);
        //}

        private static void PresentOptions(IList<Link> links, string key, int level)
        {
            if (links == null || links.Count == 0)
            {
                return;
            }

            string tabs = string.Join("", Enumerable.Repeat("\t", level).ToArray());

            System.Console.WriteLine();
            for (int l = 0; l < links.Count; l++)
            {
                var presentationKey = $"{key}.{(l + 1)}";

                System.Console.WriteLine(string.Format($"{tabs}{presentationKey}: {(!string.IsNullOrEmpty(links[l].Title) ? links[l].Title : links[l].Url)}"));
            }
        }

        private static bool CheckIfExit(Command cmd)
        {
            return cmd.Action == Action.E;
        }
    }
}
