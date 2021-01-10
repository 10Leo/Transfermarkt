using LJMB.Command;
using LJMB.Command.Commands;
using LJMB.Common;
using LJMB.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Page.Scraper.Exporter;
using Page.Scraper.Exporter.JSONExporter;
using System;
using System.Collections.Generic;
using System.Linq;
using Transfermarkt.Console.Arguments;
using Transfermarkt.Console.Options;
using Transfermarkt.Core;
using Transfermarkt.Core.Service;

namespace Transfermarkt.Console.Test
{
    class Util
    {
        public static string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);
        public static string ContinentFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ContinentFileNameFormat);
        public static string CompetitionFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.CompetitionFileNameFormat);
        public static string ClubFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ClubFileNameFormat);
        public static string OutputFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.OutputFolderPath);
        public static string Level1FolderFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.Level1FolderFormat);

        public readonly static string peekCmd = PeekCommand.KEY;
        public readonly static string parseCmd = ParseCommand.KEY;
        public readonly static string exitCmd = ExitCommand.KEY;
        public const string yearCmdOpt = "-" + YearOption.KEY;
        public const string indexesCmdOpt = "-" + IndexesOption.KEY;
        public const string exportCmdOpt = "-" + ExportOption.KEY;

        public static string FormatIndexes(List<(int i1, int i2)> opts)
        {
            return $"{string.Join(" ", opts.Select(t => string.Format("{0}.{1}", t.i1, t.i2)))}";
        }

        public static string GenerateCmd(string cmdType, List<(string k, List<string> v)> args)
        {
            string cmdToParse = $"{cmdType}";
            if (args?.Count > 0)
            {
                cmdToParse += $" {string.Join(" ", args.Select(t => string.Format("{0} {1}", t.k, string.Join(" ", t.v))))}";
            }

            return cmdToParse;
        }

        public static void Validate(string cmdToParse, ICommand cmd, string cmdType, List<(string k, List<string> v)> args, List<(int i1, int i2)> indexes)
        {
            if (args?.Count > 0)
            {
                //Assert.IsNotNull(cmd.Options, $"There should exist {args.Count} extras args.");
                //Assert.IsTrue(cmd.Options.Count == args.Count, $"Number of extra args should be {args.Count} instead of {cmd.Options.Count}.");

                ValidateYear(cmd, args);
                ValidateIndexes(cmd, args, indexes);
                ValidateExport(cmd, args);
            }
        }

        public static void ValidateYear(ICommand cmd, List<(string k, List<string> v)> args)
        {
            IEnumerable<(string k, List<string> v)> opt = args.Where(a => a.k.Trim().ToLowerInvariant() == yearCmdOpt);
            if (opt.Count() == 0)
            {
                return;
            }

            Assert.IsTrue(opt.Count() == 1, "There can exist one and only one export option.");

            (string k, List<string> v) = opt.FirstOrDefault();

            var yearOpt = cmd[YearOption.NAME] as YearOption;

            Assert.IsNotNull(yearOpt, $"{YearOption.NAME} option not found.");
            Assert.IsTrue(yearOpt.Args.Count == 1, "There must exist one and only one year option.");
            var ar = (yearOpt.Args.FirstOrDefault() as StringArgument);
            Assert.IsNotNull(ar, $"{YearOption.NAME} option didn't contain the expected argument.");
            Assert.IsTrue(ar.Value == v.FirstOrDefault().Trim().ToLowerInvariant());
        }

        public static void ValidateIndexes(ICommand cmd, List<(string k, List<string> v)> args, List<(int i1, int i2)> indexes)
        {
            IEnumerable<(string k, List<string> v)> opt = args.Where(a => a.k.Trim().ToLowerInvariant() == indexesCmdOpt);
            Assert.IsTrue(opt.Count() == 1, "There can exist one and only one indexes option.");

            (string k, List<string> v) = opt.FirstOrDefault();

            var indexesOpt = cmd[IndexesOption.NAME] as IndexesOption;

            Assert.IsNotNull(indexesOpt, $"{IndexesOption.NAME} option not found");

            for (int j = 0; j < indexes.Count; j++)
            {
                (int i1, int i2) ij = indexes[j];

                var ar = indexesOpt.Args.ElementAt(j) as Index2Argument;
                Assert.IsTrue(ar.Index1 == ij.i1 && ar.Index2 == ij.i2);
            }
        }

        public static void ValidateExport(ICommand cmd, List<(string k, List<string> v)> args)
        {
            IEnumerable<(string k, List<string> v)> opt = args.Where(a => a.k.Trim().ToLowerInvariant() == exportCmdOpt);
            if (opt.Count() == 0)
            {
                return;
            }

            Assert.IsTrue(opt.Count() == 1, "There can exist one and only one export option.");

            (string k, List<string> v) = opt.FirstOrDefault();

            var option = cmd[ExportOption.NAME] as ExportOption;
            Assert.IsNotNull(option, $"{ExportOption.NAME} option not found.");

            Assert.IsTrue(option.Args.Count == 1, "There must exist one and only one export option.");
            var ar = (option.Args.FirstOrDefault() as String2Argument);
            Assert.IsNotNull(ar, $"{ExportOption.NAME} option didn't contain the expected argument.");

            Assert.IsTrue(ar.Value == v[0].Trim().ToLowerInvariant());
            Assert.IsTrue(ar.Value2 == v[1].Trim());
        }

        public static Command GetCommand(CommandKey key, TMService TMService)
        {
            switch (key)
            {
                case CommandKey.Peek:
                    return new PeekCommand(new TMCommandProcessor(null, null, TMService));
                case CommandKey.Parse:
                    return new ParseCommand(new TMCommandProcessor(null, null, TMService));
                default:
                    return null;
            }
        }
    }

    public enum CommandKey
    {
        Peek,
        Parse
    }
}
