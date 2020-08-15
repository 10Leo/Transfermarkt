using LJMB.Command;
using LJMB.Common;
using LJMB.Logging;
using Page.Scraper.Contracts;
using Page.Scraper.Exporter;
using Page.Scraper.Exporter.JSONExporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Transfermarkt.Core;
using Transfermarkt.Core.ParseHandling.Pages;

namespace Transfermarkt.Console
{
    public class Context : IContext
    {
        private static string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);
        public static string ContinentFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ContinentFileNameFormat);
        public static string CompetitionFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.CompetitionFileNameFormat);
        public static string ClubFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ClubFileNameFormat);
        private static int MinimumLoggingLevel { get; } = ConfigManager.GetAppSetting<int>(Keys.Config.MinimumLoggingLevel);
        private static string LogPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.LogPath);
        public static string OutputFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.OutputFolderPath);
        public static string Level1FolderFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.Level1FolderFormat);

        private static readonly int currentSeason = (DateTime.Today.Month < 8) ? DateTime.Today.Year - 1 : DateTime.Today.Year;
        public bool Exit { get; set; } = false;

        public string lastSelectedSeason { get; set; } = currentSeason.ToString();

        public ILogger Logger { get; }
        public IExporter Exporter { get; }

        private readonly IList<ICommand> Commands = new List<ICommand>();

        public IDictionary<string, (Link L, ContinentPage P)> cont = new Dictionary<string, (Link, ContinentPage)>
        {
            [$"1"] = (new Link { Title = "Europe", Url = $"{BaseURL}/wettbewerbe/europa" }, null),
            [$"2"] = (new Link { Title = "America", Url = $"{BaseURL}/wettbewerbe/amerika" }, null),
            [$"3"] = (new Link { Title = "Asia", Url = $"{BaseURL}/wettbewerbe/asien" }, null),
            [$"4"] = (new Link { Title = "Africa", Url = $"{BaseURL}/wettbewerbe/afrika" }, null)
        };

        public readonly IDictionary<string, (Link L, ContinentPage P)> continent = new Dictionary<string, (Link, ContinentPage)>();

        public (Link L, ContinentPage P) Choice { get; }

        public Context()
        {
            //Commands.Add(new ExitCommand(this));
            //Commands.Add(new FetchCommand(this));
            //Commands.Add(new ParseCommand(this));

            Exporter = new JsonExporter(OutputFolderPath, Level1FolderFormat);
            Logger = LoggerFactory.GetLogger((LogLevel)MinimumLoggingLevel);
        }

        public void Run()
        {
            for (int i = 0; i < cont.Count; i++)
            {
                System.Console.WriteLine($"{(i + 1)}: {cont.ElementAt(i).Value.L.Title}");
            }

            while (!Exit)
            {
                try
                {
                    var inputCmd = GetInput();

                    if (!string.IsNullOrEmpty(inputCmd))
                    {
                        ParseAndExecuteCommand(inputCmd);
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ErrorMsg.ERROR_MSG_INTERPRET);
                    System.Console.WriteLine(ex.Message);
                }
            }
        }

        public void RegisterCommand(ICommand command)
        {
            Commands.Add(command);
        }

        private static string GetInput()
        {
            System.Console.Write("> ");
            string input = System.Console.ReadLine();
            return input;
        }

        private void ParseAndExecuteCommand(string inputCmd)
        {
            if (string.IsNullOrEmpty(inputCmd = inputCmd?.Trim()))
            {
                throw new Exception(ErrorMsg.ERROR_MSG_CMD);
            }

            var cmdGroup = "CMD";
            var m = Regex.Matches(inputCmd, $@"^(?<{cmdGroup}>\w)\s*");

            if (m == null || m.Count == 0)
            {
                throw new Exception(ErrorMsg.ERROR_MSG_CMD);
            }

            var cm = m[0].Groups[cmdGroup];
            if (cm == null || string.IsNullOrEmpty(cm.Value) || string.IsNullOrWhiteSpace(cm.Value))
            {
                throw new Exception(ErrorMsg.ERROR_MSG_CMD);
            }


            ICommand sentCmd = null;
            foreach (ICommand c in Commands)
            {
                if (c.CanParse(cm.Value.Trim().ToLowerInvariant()))
                {
                    sentCmd = c;
                    break;
                }
            }

            if (sentCmd == null)
            {
                throw new Exception(ErrorMsg.ERROR_MSG_CMD_NOT_FOUND);
            }

            sentCmd.Parse(inputCmd);
            sentCmd.Execute();
        }

        public static void PresentOptions(IList<Link> links, string key, int level)
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
    }
}
