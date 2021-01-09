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
        private static readonly string tabsContinent = string.Join("", Enumerable.Repeat("\t", 1).ToArray());
        private static readonly string tabsCompetitions = string.Join("", Enumerable.Repeat("\t", 2).ToArray());
        private static readonly string tabsClubs = string.Join("", Enumerable.Repeat("\t", 3).ToArray());

        private readonly string ContinentsFormat = tabsContinent + "{0,2}: {1}";
        private readonly string CompetitionsFormat = tabsCompetitions + "{0,2}.{1,2}: {2}";
        private readonly string ClubsFormat = tabsClubs + "{0,2}.{1,2}.{2,2}: {3}";

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

            this.RegisterCommand(new ExitCommand(this));
            this.RegisterCommand(new ListCommand(this));
            this.RegisterCommand(new PeekCommand(this));

            var p = new ParseCommand(this)
            {
                Exporters = exporters,
                ContinentFileNameFormat = ContinentFileNameFormat,
                CompetitionFileNameFormat = CompetitionFileNameFormat,
                ClubFileNameFormat = ClubFileNameFormat
            };
            this.RegisterCommand(p);

            GetCommands = () => CollectCommands();
        }

        public override void Run()
        {
            PrintOptions(currentSeason);

            base.Run();
        }

        public void PrintOptions(int? year)
        {
            //System.Console.WriteLine();

            if (year.HasValue)
            {
                System.Console.WriteLine($"Season: {year}");
            }

            if (TMService.SeasonContinents.ContainsKey(year.Value))
            {
                ContinentsPage scPage = TMService.SeasonContinents[year.Value];
                PrintChilds(scPage);
            }
        }

        private void PrintChilds(ContinentsPage page)
        {
            var continentsSection = (ContinentsContinentsPageSection)page[ContinentsContinentsPageSection.SectionName];

            int i1 = 0;
            foreach (Link<HtmlNode, ContinentPage> continent in continentsSection.Children)
            {
                i1++;
                System.Console.WriteLine(string.Format(ContinentsFormat, i1, continent.Title));

                if (continent.Page == null)
                {
                    continue;
                }

                var allSectionsOfTypeChildSections = continent.Page.Sections.OfType<ChildsSection<HtmlNode, CompetitionPage>>();
                if (allSectionsOfTypeChildSections == null || allSectionsOfTypeChildSections.Count() == 0)
                {
                    return;
                }

                foreach (ChildsSection<HtmlNode, CompetitionPage> sectionOfTypeChildSection in allSectionsOfTypeChildSections)
                {
                    int i2 = 0;
                    foreach (Link<HtmlNode, CompetitionPage> competition in sectionOfTypeChildSection.Children)
                    {
                        i2++;
                        System.Console.WriteLine(string.Format(CompetitionsFormat, i1, i2, competition.Title));

                        if (competition.Page == null)
                        {
                            continue;
                        }

                        var allCompetitionsSectionsOfTypeChildSections = competition.Page.Sections.OfType<ChildsSection<HtmlNode, ClubPage>>();

                        if (allCompetitionsSectionsOfTypeChildSections == null || allCompetitionsSectionsOfTypeChildSections.Count() == 0)
                        {
                            break;
                        }

                        foreach (ChildsSection<HtmlNode, ClubPage> competitionSectionOfTypeChildSection in allCompetitionsSectionsOfTypeChildSections)
                        {
                            int i3 = 0;
                            foreach (Link<HtmlNode, ClubPage> club in competitionSectionOfTypeChildSection.Children)
                            {
                                i3++;
                                System.Console.WriteLine(string.Format(ClubsFormat, i1, i2, i3, club.Title));
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<string> CollectCommands()
        {
            while (!Exit)
            {
                yield return GetInput();
            }
        }

        private string GetInput()
        {
            System.Console.WriteLine();
            System.Console.Write("> ");
            string input = System.Console.ReadLine();
            return input;
        }
    }
}
