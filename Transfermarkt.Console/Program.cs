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

        private static IDictionary<string, (Link L, ContinentPage P)> continent = new Dictionary<string, (Link, ContinentPage)>
        {
            ["1"] = (new Link { Title = "Europe", Url = $"{BaseURL}/wettbewerbe/europa" }, null),
            ["2"] = (new Link { Title = "America", Url = $"{BaseURL}/wettbewerbe/amerika" }, null),
            ["3"] = (new Link { Title = "Asia", Url = $"{BaseURL}/wettbewerbe/asien" }, null),
            ["4"] = (new Link { Title = "Africa", Url = $"{BaseURL}/wettbewerbe/afrika" }, null)
        };
        private static IDictionary<string, (Link L, CompetitionPage P)> competition = new Dictionary<string, (Link, CompetitionPage)>();
        private static IDictionary<string, (Link L, ClubPage P)> club = new Dictionary<string, (Link L, ClubPage P)>();

        static void Main(string[] args)
        {
            pageTypes.Add(2, typeof(ContinentPage));
            pageTypes.Add(3, typeof(CompetitionPage));
            pageTypes.Add(4, typeof(ClubPage));
            exporter = new JsonExporter();

            System.Console.WriteLine("Transfermarkt Web Scrapper\n");

            PresentOptions(continent);

            bool exit = false;
            while (!exit)
            {
                try
                {
                    Command cmd = GetInput();

                    exit = CheckIfExit(cmd);

                    if (!exit)
                    {
                        Ac(cmd);
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
            string input = System.Console.ReadLine();
            return CommandUtil.ParseCommand(input);
        }

        private static void Ac(Command cmd)
        {
            (ParameterName Cmd, IParameterValue Val) y = cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.Y);

            IndexesParameterValue i = cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.I).Val as IndexesParameterValue;
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

                bool proceed = true;
                if (proceed && i1 != 0)
                {
                    proceed = ContinentP(cmd, i1.ToString(), ind is Index1ParameterValue);
                }
                if (proceed && i2 != 0)
                {
                    proceed = CompetitionP(cmd, $"{i1.ToString()}.{i2.ToString()}", ind is Index2ParameterValue);
                }
                if (proceed && i3 != 0)
                {
                    proceed = ClubP(cmd, $"{i1.ToString()}.{i2.ToString()}.{i3.ToString()}", ind is Index3ParameterValue);
                }
            }
        }

        private static void PresentOptions(IDictionary<string, (Link L, ContinentPage P)> urls)
        {
            System.Console.WriteLine("Escolha uma das seguintes opções:");
            System.Console.WriteLine(string.Format("0: Todas"));
            for (int i = 0; i < urls.Count; i++)
            {
                var v = urls.Keys.ElementAt(i);
                System.Console.WriteLine(string.Format("{0}: {1}", v, (!string.IsNullOrEmpty(urls[v].L.Title) ? urls[v].L.Title : urls[v].L.Url)));

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

        private static void PresentOptions2(string index, IDictionary<string, (Link L, ContinentPage P)> urls, IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> childsSection)
        {
            for (int l = 0; l < childsSection.Children.Count; l++)
            {
                var key = $"{index + "." + (l + 1)}";

                System.Console.WriteLine(string.Format("\t{0}: {1}", key, (!string.IsNullOrEmpty(urls[key].L.Title) ? urls[key].L.Title : urls[key].L.Url)));
            }
        }

        private static bool ContinentP(Command cmd, String index, bool isFinal)
        {
            if (!continent.ContainsKey(index.ToString()))
            {
                return false;
            }

            IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> childLinksSection = null;
            (Link L, ContinentPage P) choice = continent[index.ToString()];
            if (choice.P == null)
            {
                choice.P = (ContinentPage)Activator.CreateInstance(pageTypes[2], new HAPConnection(), logger);
                continent[index.ToString()] = choice;

                choice.P.Fetch(choice.L.Url);

                childLinksSection = choice.P.Sections.OfType<IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>>().FirstOrDefault(s => s.Name == "Competitions");
                for (int l = 0; l < childLinksSection.Children.Count; l++)
                {
                    var key = $"{index}.{(l + 1)}";

                    if (!competition.ContainsKey(key))
                    {
                        competition.Add(key, (childLinksSection.Children[l], null));
                    }
                }
            }

            if (!isFinal)
            {
                return true;
            }

            if (cmd.CommandType == CommandType.P)
            {
                choice.P.Parse(choice.L.Url);
                exporter.Extract(choice.P.Domain);
            }

            if (childLinksSection == null)
            {
                childLinksSection = choice.P.Sections.OfType<IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>>().FirstOrDefault(s => s.Name == "Competitions");
            }

            // Present options
            System.Console.WriteLine();
            for (int l = 0; l < childLinksSection.Children.Count; l++)
            {
                var key = $"{index + "." + (l + 1)}";

                System.Console.WriteLine(string.Format("\t{0}: {1}", key, (!string.IsNullOrEmpty(competition[key].L.Title) ? competition[key].L.Title : competition[key].L.Url)));
            }

            return true;
        }

        private static bool CompetitionP(Command cmd, String index, bool isFinal)
        {

            if (!competition.ContainsKey(index.ToString()))
            {
                return false;
            }

            IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> childLinksSection = null;
            (Link L, CompetitionPage P) choice = competition[index.ToString()];
            if (choice.P == null)
            {
                choice.P = (CompetitionPage)Activator.CreateInstance(pageTypes[3], new HAPConnection(), logger);
                competition[index.ToString()] = choice;

                choice.P.Fetch(choice.L.Url);

                childLinksSection = choice.P.Sections.OfType<IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>>().FirstOrDefault(s => s.Name == "Clubs");
                for (int l = 0; l < childLinksSection.Children.Count; l++)
                {
                    var key = $"{index}.{(l + 1)}";

                    if (!club.ContainsKey(key))
                    {
                        club.Add(key, (childLinksSection.Children[l], null));
                    }
                }
            }

            if (!isFinal)
            {
                return true;
            }

            if (cmd.CommandType == CommandType.P)
            {
                choice.P.Parse(choice.L.Url);
                exporter.Extract(choice.P.Domain);
            }

            if (childLinksSection == null)
            {
                childLinksSection = choice.P.Sections.OfType<IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>>().FirstOrDefault(s => s.Name == "Competitions");
            }

            // Present options
            System.Console.WriteLine();
            for (int l = 0; l < childLinksSection.Children.Count; l++)
            {
                var key = $"{index + "." + (l + 1)}";

                System.Console.WriteLine(string.Format("\t{0}: {1}", key, (!string.IsNullOrEmpty(club[key].L.Title) ? club[key].L.Title : club[key].L.Url)));
            }

            return true;
        }

        private static bool ClubP(Command cmd, String index, bool isFinal)
        {

            if (!club.ContainsKey(index.ToString()))
            {
                return false;
            }

            IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> childLinksSection = null;
            (Link L, ClubPage P) choice = club[index.ToString()];
            if (choice.P == null)
            {
                choice.P = (ClubPage)Activator.CreateInstance(pageTypes[4], new HAPConnection(), logger);
                club[index.ToString()] = choice;

                choice.P.Fetch(choice.L.Url);
            }

            if (!isFinal)
            {
                return true;
            }

            if (cmd.CommandType == CommandType.P)
            {
                choice.P.Parse(choice.L.Url);
                exporter.Extract(choice.P.Domain);
            }

            return true;
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

        private static bool CheckIfExit(Command cmd)
        {
            return cmd.CommandType == CommandType.E;
        }
    }
}
