using HtmlAgilityPack;
using LJMB.Command;
using LJMB.Command.Commands;
using LJMB.Common;
using LJMB.Logging;
using Page.Scraper.Contracts;
using Page.Scraper.Exporter;
using Page.Scraper.Exporter.JSONExporter;
using System;
using System.Collections.Generic;
using System.Linq;
using Transfermarkt.Core;
using Transfermarkt.Core.ParseHandling.Pages;
using Transfermarkt.Core.Service;

namespace Transfermarkt.Console
{
    public class TMContext : Context
    {
        private static readonly int currentSeason = (DateTime.Today.Month < 8) ? DateTime.Today.Year - 1 : DateTime.Today.Year;
        public string LastSelectedSeason { get; set; } = currentSeason.ToString(); 
        private static int MinimumLoggingLevel { get; } = ConfigManager.GetAppSetting<int>(Keys.Config.MinimumLoggingLevel);
        private static string LogPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.LogPath);
        public static string OutputFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.OutputFolderPath);
        public static string Level1FolderFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.Level1FolderFormat);

        public ILogger Logger { get; }
        public IExporter Exporter { get; }
        public TMService TMService { get; set; }

        public TMContext()
        {
            this.RegisterCommand(new ExitCommand(this));
            this.RegisterCommand(new PeekCommand(this));
            this.RegisterCommand(new ParseCommand(this));

            Exporter = new JsonExporter(OutputFolderPath, Level1FolderFormat);
            Logger = LoggerFactory.GetLogger((LogLevel)MinimumLoggingLevel);
            TMService = new TMService();
        }

        public override void Run()
        {
            //for (int i = 0; i < Continents.Count; i++)
            //{
            //    System.Console.WriteLine($"{(i + 1)}: {Continents.ElementAt(i).Value.L.Title}");
            //}

            base.Run();
        }

        public static void PresentOptions(IList<Link<HtmlNode, CompetitionPage>> links, string key, int level)
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

        public static void PresentOptions(IList<Link<HtmlNode, ClubPage>> links, string key, int level)
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
