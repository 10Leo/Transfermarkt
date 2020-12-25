﻿using HtmlAgilityPack;
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

        public ILogger Logger { get; }
        public IExporter Exporter { get; }
        public TMService TMService { get; set; }

        public TMContext(ILogger logger, IExporter exporter, TMService tmService)
        {
            this.RegisterCommand(new ExitCommand(this));
            this.RegisterCommand(new PeekCommand(this));
            this.RegisterCommand(new ParseCommand(this));

            this.Logger = logger;
            this.Exporter = exporter;
            this.TMService = tmService;
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
