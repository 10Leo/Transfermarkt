using HtmlAgilityPack;
using LJMB.Command;
using LJMB.Command.Commands;
using LJMB.Logging;
using Page.Scraper.Contracts;
using Page.Scraper.Exporter;
using System;
using System.Collections.Generic;
using System.Linq;
using Transfermarkt.Core;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Pages;
using Transfermarkt.Core.Service;

namespace Transfermarkt.Console
{
    public class TMCommandProcessor : Processor
    {
        public ILogger Logger { get; }
        public IDictionary<ExportType, IExporter> Exporters { get; }
        public TMService TMService { get; set; }
        public string LastSelectedSeason { get; set; }
        public static string ContinentFileNameFormat { get; set; }
        public static string CompetitionFileNameFormat { get; set; }
        public static string ClubFileNameFormat { get; set; }

        private static readonly int currentSeason = (DateTime.Today.Month < 8) ? DateTime.Today.Year - 1 : DateTime.Today.Year;

        public TMCommandProcessor(ILogger logger, IDictionary<ExportType, IExporter> exporters, TMService tmService)
        {
            this.Logger = logger;
            this.Exporters = exporters;
            this.TMService = tmService;
            this.LastSelectedSeason = currentSeason.ToString();

            var p = new ParseCommand(this)
            {
                Exporters = exporters,
                ContinentFileNameFormat = ContinentFileNameFormat,
                CompetitionFileNameFormat = CompetitionFileNameFormat,
                ClubFileNameFormat = ClubFileNameFormat
            };
            this.RegisterCommand(new ExitCommand(this));
            this.RegisterCommand(new ListCommand(this));
            this.RegisterCommand(new PeekCommand(this));
            this.RegisterCommand(p);
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

            foreach (KeyValuePair<ContinentCode, Func<Link<HtmlNode, ContinentPage>>> kvp in TMService.Continents)
            {
                //TODO: modify so that the Func.Invoke call isn't used unnecessarily
                System.Console.WriteLine($"{tabsContinent}{(int)kvp.Key}: {kvp.Value.Invoke().Title}");

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
    }
}
