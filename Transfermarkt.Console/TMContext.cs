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
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Pages;
using Transfermarkt.Core.Service;

namespace Transfermarkt.Console
{
    public class TMContext : Context
    {
        private static readonly int currentSeason = (DateTime.Today.Month < 8) ? DateTime.Today.Year - 1 : DateTime.Today.Year;

        public string LastSelectedSeason { get; set; }
        public ILogger Logger { get; }
        public IExporter Exporter { get; }
        public TMService TMService { get; set; }

        public TMContext(ILogger logger, IExporter exporter, TMService tmService)
        {
            this.Logger = logger;
            this.Exporter = exporter;
            this.TMService = tmService;
            this.LastSelectedSeason = currentSeason.ToString();

            this.RegisterCommand(new ExitCommand(this));
            this.RegisterCommand(new PeekCommand(this));
            this.RegisterCommand(new ParseCommand(this));
        }

        public override void Run()
        {
            PrintOptions(currentSeason);

            base.Run();
        }

        public void PrintOptions(int? year)
        {
            string tabsContinent = string.Join("", Enumerable.Repeat("\t", 1).ToArray());

            System.Console.WriteLine();

            if (year.HasValue)
            {
                System.Console.WriteLine($"Season: {year}");
            }

            foreach (KeyValuePair<ContinentCode, Link<HtmlNode, ContinentPage>> kvp in TMService.Continents)
            {
                System.Console.WriteLine($"{tabsContinent}{(int)kvp.Key}: {kvp.Value.Title}");

                var key = string.Format(TMService.KEY_PATTERN, year, (int)kvp.Key);
                if (TMService.SeasonContinents.ContainsKey(key))
                {
                    Link<HtmlNode, ContinentPage> choice = TMService.SeasonContinents[key];
                    PrintChilds(choice.Page);
                }
            }
        }

        private void PrintChilds(ContinentPage page)
        {
            string tabsCompetitions = string.Join("", Enumerable.Repeat("\t", 2).ToArray());
            string tabsClubs = string.Join("", Enumerable.Repeat("\t", 3).ToArray());

            var allSectionsOfTypeChildSections = page.Sections.OfType<ChildsSection<HtmlNode, CompetitionPage>>();

            if (allSectionsOfTypeChildSections == null || allSectionsOfTypeChildSections.Count() == 0)
            {
                return;
            }

            foreach (ChildsSection<HtmlNode, CompetitionPage> sectionOfTypeChildSection in allSectionsOfTypeChildSections)
            {
                int i0 = 1;
                foreach (Link<HtmlNode, CompetitionPage> continentChild in sectionOfTypeChildSection.Children)
                {
                    System.Console.WriteLine($"{tabsCompetitions}{i0++}: {continentChild.Title}");

                    if (continentChild.Page == null)
                    {
                        continue;
                    }

                    var allCompetitionsSectionsOfTypeChildSections = continentChild.Page.Sections.OfType<ChildsSection<HtmlNode, ClubPage>>();

                    if (allCompetitionsSectionsOfTypeChildSections == null || allCompetitionsSectionsOfTypeChildSections.Count() == 0)
                    {
                        break;
                    }

                    int i1 = 1;
                    foreach (ChildsSection<HtmlNode, ClubPage> competitionSectionOfTypeChildSection in allCompetitionsSectionsOfTypeChildSections)
                    {
                        foreach (Link<HtmlNode, ClubPage> competitionChild in competitionSectionOfTypeChildSection.Children)
                        {
                            System.Console.WriteLine($"{tabsClubs}{i1++}: {competitionChild.Title}");
                        }
                    }
                }
            }
        }

        private void PrintContinentOptions()
        {
            for (int i = 0; i < TMService.Continents.Count; i++)
            {
                System.Console.WriteLine($"{(i + 1)}: {TMService.Continents.ElementAt(i).Value.Title}");
            }
        }

        public static void PrintOptions(IList<Link<HtmlNode, CompetitionPage>> links, string key, int level)
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

        public static void PrintOptions(IList<Link<HtmlNode, ClubPage>> links, string key, int level)
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
