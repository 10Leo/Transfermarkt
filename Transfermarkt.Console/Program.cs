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

        private static readonly ILogger logger = LoggerFactory.GetLogger((LogLevel)MinimumLoggingLevel);

        private static IExporter exporter;

        private static int currentSeason = (DateTime.Today.Month < 8) ? DateTime.Today.Year - 1 : DateTime.Today.Year;
        private static string lastSelectedSeason = currentSeason.ToString();

        private static IDictionary<int, Type> pageTypes = new Dictionary<int, Type>();

        private static IDictionary<string, (Link L, ContinentPage P)> cont = new Dictionary<string, (Link, ContinentPage)>
        {
            [$"1"] = (new Link { Title = "Europe", Url = $"{BaseURL}/wettbewerbe/europa" }, null),
            [$"2"] = (new Link { Title = "America", Url = $"{BaseURL}/wettbewerbe/amerika" }, null),
            [$"3"] = (new Link { Title = "Asia", Url = $"{BaseURL}/wettbewerbe/asien" }, null),
            [$"4"] = (new Link { Title = "Africa", Url = $"{BaseURL}/wettbewerbe/afrika" }, null)
        };

        private static readonly IDictionary<string, (Link L, ContinentPage P)> continent = new Dictionary<string, (Link, ContinentPage)>();
        private static readonly IDictionary<string, (Link L, CompetitionPage P)> competition = new Dictionary<string, (Link, CompetitionPage)>();
        private static readonly IDictionary<string, (Link L, ClubPage P)> club = new Dictionary<string, (Link L, ClubPage P)>();


        static void Main(string[] args)
        {
            //pageTypes.Add(2, typeof(ContinentPage));
            //pageTypes.Add(3, typeof(CompetitionPage));
            //pageTypes.Add(4, typeof(ClubPage));
            //exporter = new JsonExporter();

            //System.Console.WriteLine("Transfermarkt Web Scrapper\n");
            

            //for (int i = 0; i < cont.Count; i++)
            //{
            //    System.Console.WriteLine($"{(i + 1)}: {cont.ElementAt(i).Value}");
            //}

            //bool exit = false;
            //while (!exit)
            //{
            //    try
            //    {
            //        Command cmd = GetInput();

            //        exit = CheckIfExit(cmd);

            //        if (!exit)
            //        {
            //            Proccess(cmd);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Console.WriteLine("Error reading or interpreting chosen option.");
            //        System.Console.WriteLine(ex.Message);
            //    }
            //}
        }

        //private static Command GetInput()
        //{
        //    System.Console.Write("> ");
        //    string input = System.Console.ReadLine();
        //    return CommandUtil.ParseCommand(input);
        //}

        //private static void Proccess(Command cmd)
        //{
        //    IndexesParameterValue i = cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.I).Val as IndexesParameterValue;

        //    // Check if a year was passed by the user as an argument. If not get the last passed one, or the current one, if one was not passed yet. 
        //    var yy = cmd[ParameterName.Y];
        //    if (yy == null)
        //    {
        //        var y = new StringParameterValue
        //        {
        //            Value = lastSelectedSeason
        //        };
        //        cmd.Parameters.Add((ParameterName.Y, y));
        //    }

        //    lastSelectedSeason = ((StringParameterValue)cmd[ParameterName.Y]).Value;

        //    foreach (IIndex ind in i.Indexes)
        //    {
        //        int i1 = 0;
        //        int i2 = 0;
        //        int i3 = 0;

        //        if (ind is Index1ParameterValue)
        //        {
        //            i1 = (ind as Index1ParameterValue).Index1;
        //        }
        //        else if (ind is Index2ParameterValue)
        //        {
        //            i1 = (ind as Index2ParameterValue).Index1;
        //            i2 = (ind as Index2ParameterValue).Index2;
        //        }
        //        else if (ind is Index3ParameterValue)
        //        {
        //            i1 = (ind as Index3ParameterValue).Index1;
        //            i2 = (ind as Index3ParameterValue).Index2;
        //            i3 = (ind as Index3ParameterValue).Index3;
        //        }

        //        bool proceed = ContinentsP(cmd, $"{i1.ToString()}");
        //        if (proceed && i1 != 0)
        //        {
        //            proceed = ContinentP(cmd, $"{i1.ToString()}", ind is Index1ParameterValue);
        //        }
        //        if (proceed && i2 != 0)
        //        {
        //            proceed = CompetitionP(cmd, $"{i1.ToString()}.{i2.ToString()}", ind is Index2ParameterValue);
        //        }
        //        if (proceed && i3 != 0)
        //        {
        //            proceed = ClubP(cmd, $"{i1.ToString()}.{i2.ToString()}.{i3.ToString()}", ind is Index3ParameterValue);
        //        }
        //    }
        //}

        //private static bool ContinentsP(Command cmd, string index)
        //{
        //    var year = ((StringParameterValue)cmd[ParameterName.Y]).Value;

        //    var k = $"{year}.{index}";

        //    if (!cont.ContainsKey(index) )
        //    {
        //        return false;
        //    }

        //    if (!continent.ContainsKey(k))
        //    {
        //        continent.Add(k, (cont[index].L, null));
        //    }

        //    return true;
        //}

        //private static bool ContinentP(Command cmd, string index, bool isFinal)
        //{
        //    var year = ((StringParameterValue)cmd[ParameterName.Y]).Value;

        //    var k = $"{year}.{index}";

        //    if (!continent.ContainsKey(k))
        //    {
        //        return false;
        //    }

        //    IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> childLinksSection = null;
        //    (Link L, ContinentPage P) choice = continent[k];
        //    if (choice.P == null)
        //    {
        //        choice.P = (ContinentPage)Activator.CreateInstance(pageTypes[2], new HAPConnection(), logger, year);
        //        continent[k] = choice;

        //        choice.P.Fetch(choice.L.Url);

        //        childLinksSection = choice.P.Sections.OfType<IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>>().FirstOrDefault(s => s.Name == "Continent - Competitions Section");
        //        for (int l = 0; l < childLinksSection.Children.Count; l++)
        //        {
        //            var key = $"{year}.{index}.{(l + 1)}";

        //            if (!competition.ContainsKey(key))
        //            {
        //                competition.Add(key, (childLinksSection.Children[l], null));
        //            }
        //        }
        //    }

        //    if (!isFinal)
        //    {
        //        return true;
        //    }

        //    if (cmd.Action == Action.P)
        //    {
        //        choice.P.Parse(choice.L.Url);
        //        exporter.Extract(choice.P.Domain);
        //    }

        //    if (childLinksSection == null)
        //    {
        //        childLinksSection = choice.P.Sections.OfType<IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>>().FirstOrDefault(s => s.Name == "Continent - Competitions Section");
        //    }

        //    if (childLinksSection == null)
        //    {
        //        return true;
        //    }

        //    // Present options
        //    System.Console.WriteLine();
        //    for (int l = 0; l < childLinksSection.Children.Count; l++)
        //    {
        //        var presentationKey = $"{index}.{(l + 1)}";
        //        var key = $"{k}.{(l + 1)}";

        //        System.Console.WriteLine(string.Format("\t{0}: {1}", presentationKey, (!string.IsNullOrEmpty(competition[key].L.Title) ? competition[key].L.Title : competition[key].L.Url)));
        //    }

        //    return true;
        //}

        //private static bool CompetitionP(Command cmd, string index, bool isFinal)
        //{
        //    var year = ((StringParameterValue)cmd[ParameterName.Y]).Value;

        //    var k = $"{year}.{index}";
            
        //    if (!competition.ContainsKey(k))
        //    {
        //        return false;
        //    }

        //    IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> childLinksSection = null;
        //    (Link L, CompetitionPage P) choice = competition[k];
        //    if (choice.P == null)
        //    {
        //        choice.P = (CompetitionPage)Activator.CreateInstance(pageTypes[3], new HAPConnection(), logger, year);
        //        competition[k] = choice;

        //        choice.P.Fetch(choice.L.Url);

        //        childLinksSection = choice.P.Sections.OfType<IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>>().FirstOrDefault(s => s.Name == "Competition - Clubs Section");
        //        for (int l = 0; l < childLinksSection.Children.Count; l++)
        //        {
        //            var key = $"{k}.{(l + 1)}";

        //            if (!club.ContainsKey(key))
        //            {
        //                club.Add(key, (childLinksSection.Children[l], null));
        //            }
        //        }
        //    }

        //    if (!isFinal)
        //    {
        //        return true;
        //    }

        //    if (cmd.Action == Action.P)
        //    {
        //        choice.P.Parse(choice.L.Url);
        //        exporter.Extract(choice.P.Domain);
        //    }

        //    if (childLinksSection == null)
        //    {
        //        childLinksSection = choice.P.Sections.OfType<IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>>().FirstOrDefault(s => s.Name == "Competition - Clubs Section");
        //    }

        //    if (childLinksSection == null)
        //    {
        //        return true;
        //    }

        //    // Present options
        //    System.Console.WriteLine();
        //    for (int l = 0; l < childLinksSection.Children.Count; l++)
        //    {
        //        var presentationKey = $"{index}.{(l + 1)}";
        //        var key = $"{k}.{(l + 1)}";

        //        System.Console.WriteLine(string.Format("\t{0}: {1}", presentationKey, (!string.IsNullOrEmpty(club[key].L.Title) ? club[key].L.Title : club[key].L.Url)));
        //    }

        //    return true;
        //}

        //private static bool ClubP(Command cmd, string index, bool isFinal)
        //{
        //    var year = ((StringParameterValue)cmd[ParameterName.Y]).Value;

        //    var k = $"{year}.{index}";
            
        //    if (!club.ContainsKey(k))
        //    {
        //        return false;
        //    }

        //    IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> childLinksSection = null;
        //    (Link L, ClubPage P) choice = club[k];
        //    if (choice.P == null)
        //    {
        //        choice.P = (ClubPage)Activator.CreateInstance(pageTypes[4], new HAPConnection(), logger, year);
        //        club[k] = choice;

        //        choice.P.Fetch(choice.L.Url);
        //    }

        //    if (!isFinal)
        //    {
        //        return true;
        //    }

        //    if (cmd.Action == Action.P)
        //    {
        //        choice.P.Parse(choice.L.Url);
        //        exporter.Extract(choice.P.Domain);
        //    }

        //    return true;
        //}

        //private static void PresentOptions(IDictionary<string, (Link L, ContinentPage P)> urls)
        //{
        //    System.Console.WriteLine("Escolha uma das seguintes opções:");
        //    for (int i = 0; i < urls.Count; i++)
        //    {
        //        var v = urls.Keys.ElementAt(i);
        //        System.Console.WriteLine(string.Format("{0}: {1}", v, (!string.IsNullOrEmpty(urls[v].L.Title) ? urls[v].L.Title : urls[v].L.Url)));
        //    }
        //}

        //private static void PresentOptions2(string index, IDictionary<string, (Link L, ContinentPage P)> urls, IChildsSection<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> childsSection)
        //{
        //    for (int l = 0; l < childsSection.Children.Count; l++)
        //    {
        //        var key = $"{index + "." + (l + 1)}";

        //        System.Console.WriteLine(string.Format("\t{0}: {1}", key, (!string.IsNullOrEmpty(urls[key].L.Title) ? urls[key].L.Title : urls[key].L.Url)));
        //    }
        //}

        //private static bool CheckIfExit(Command cmd)
        //{
        //    return cmd.Action == Action.E;
        //}
    }
}
