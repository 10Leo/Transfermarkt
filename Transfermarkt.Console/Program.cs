﻿using HtmlAgilityPack;
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
                        (ParameterName Cmd, IParameterValue Val) y = cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.Y);


                        IndexesParameterValue i = cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.I).Val as IndexesParameterValue;

                        foreach (IIndex ind in i.Indexes)
                        {
                            if (ind is Index1ParameterValue)
                            {
                                int index = (ind as Index1ParameterValue).Index1;

                                if (!continent.ContainsKey(index.ToString()))
                                {
                                    continue;
                                }

                                (Link L, ContinentPage P) choice = continent[index.ToString()];

                                if (choice.P != null)
                                {
                                    continue;
                                }

                                (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> Page, List<Link> Links) e = ExecuteAction(cmd, pageTypes[2], choice.L.Url);
                                choice.P = (ContinentPage)e.Page;
                                continent[index.ToString()] = choice;

                                for (int l = 0; l < e.Links.Count; l++)
                                {
                                    var key = $"{index + "." + (l + 1)}";

                                    if (!competition.ContainsKey(key))
                                    {
                                        competition.Add(key, (e.Links[l], null));
                                    }
                                }

                                // Present options
                                System.Console.WriteLine();
                                for (int l = 0; l < e.Links.Count; l++)
                                {
                                    var key = $"{index + "." + (l + 1)}";

                                    System.Console.WriteLine(string.Format("\t{0}: {1}", key, (!string.IsNullOrEmpty(competition[key].L.Title) ? competition[key].L.Title : competition[key].L.Url)));
                                }
                            }
                            else if (ind is Index2ParameterValue)
                            {
                                int Index1 = (ind as Index2ParameterValue).Index1;
                                int Index2 = (ind as Index2ParameterValue).Index2;
                                string index = $"{Index1 + "." + Index2}";

                                if (!competition.ContainsKey(index.ToString()))
                                {
                                    continue;
                                }

                                (Link L, CompetitionPage P) choice = competition[index.ToString()];

                                if (choice.P != null)
                                {
                                    continue;
                                }

                                (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> Page, List<Link> Links) e = ExecuteAction(cmd, pageTypes[3], choice.L.Url);
                                choice.P = (CompetitionPage)e.Page;
                                competition[index.ToString()] = choice;

                                for (int l = 0; l < e.Links.Count; l++)
                                {
                                    var key = $"{Index1 + "." + Index2 + "." + (l + 1)}";

                                    if (!club.ContainsKey(key))
                                    {
                                        club.Add(key, (e.Links[l], null));
                                    }
                                }

                                // Present options
                                System.Console.WriteLine();
                                for (int l = 0; l < e.Links.Count; l++)
                                {
                                    var key = $"{index + "." + (l + 1)}";

                                    System.Console.WriteLine(string.Format("\t{0}: {1}", key, (!string.IsNullOrEmpty(club[key].L.Title) ? club[key].L.Title : club[key].L.Url)));
                                }
                            }
                            else if (ind is Index3ParameterValue)
                            {
                                int Index1 = (ind as Index3ParameterValue).Index1;
                                int Index2 = (ind as Index3ParameterValue).Index2;
                                int Index3 = (ind as Index3ParameterValue).Index3;
                                string index = $"{Index1 + "." + Index2 + "." + Index3}";

                                if (!club.ContainsKey(index.ToString()))
                                {
                                    continue;
                                }

                                (Link L, ClubPage P) choice = club[index.ToString()];

                                if (choice.P != null)
                                {
                                    continue;
                                }

                                (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> Page, List<Link> Links) e = ExecuteAction(cmd, pageTypes[4], choice.L.Url);
                                choice.P = (ClubPage)e.Page;
                                club[index.ToString()] = choice;
                            }
                        }
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

        private static void Execute(Command cmd, Type type, IDictionary<string, Link> urls)
        {
            var pages = new List<IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode>>();
            //var childUrlsCollection = new Dictionary<string, Link>();

            IndexesParameterValue fir = cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.I).Val as IndexesParameterValue;
            foreach (var ind in fir.Indexes)
            {
                if (ind is Index1ParameterValue)
                {
                    int index = ((Index1ParameterValue)cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.I).Val).Index1;

                    {
                        string choice = $"{urls[index.ToString()].Url}";

                        (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> Page, List<Link> Links) e = ExecuteAction(cmd, type, choice);
                        pages.Add(e.Page);

                        for (int i = 0; i < e.Links.Count; i++)
                        {
                            //competition.Add($"{index + "." + (i + 1)}", e.Links[i]);
                        }
                    }
                }
                else if (ind is Index2ParameterValue)
                {
                    var f = ((Index2ParameterValue)cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.I).Val);
                    int Index1 = f.Index1;
                    int Index2 = f.Index2;

                    {
                        string choice = $"{urls[Index1 + "." + Index2].Url}";

                        (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> Page, List<Link> Links) e = ExecuteAction(cmd, type, choice);
                        pages.Add(e.Page);

                        for (int i = 0; i < e.Links.Count; i++)
                        {
                            club.Add($"{Index2 + "." + (i + 1)}", (e.Links[i], null));
                        }
                    }
                }
                //else if (ind is Index3ParameterValue)
                //{
                //    var f = ((Index3ParameterValue)cmd.Parameters.FirstOrDefault(a => a.Cmd == ParameterName.O).Val);
                //    int Index1 = f.Index1;
                //    int Index2 = f.Index2;
                //    int Index3 = f.Index3;

                //    string choice = $"{urls[Index1 + "." + Index2 + "." + Index3].Url}";

                //    (IPage<IDomain<IValue>, IElement<IValue>, IValue, HtmlNode> Page, List<Link> Links) e = ExecuteAction(cmd, type, choice);
                //    pages.Add(e.Page);

                //    for (int i = 0; i < e.Links.Count; i++)
                //    {
                //        club.Add($"{Index2 + "." + (i + 1)}", e.Links[i]);
                //    }
                //}
            }
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
