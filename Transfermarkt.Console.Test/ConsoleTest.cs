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
    [TestClass]
    public class ConsoleTest
    {
        protected static string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);
        protected static string ContinentFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ContinentFileNameFormat);
        protected static string CompetitionFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.CompetitionFileNameFormat);
        protected static string ClubFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ClubFileNameFormat);
        protected static string OutputFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.OutputFolderPath);
        protected static string Level1FolderFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.Level1FolderFormat);

        private readonly static string peekCmd = PeekCommand.KEY;
        private readonly static string parseCmd = ParseCommand.KEY;
        private readonly static string exitCmd = ExitCommand.KEY;
        private const string yearCmdOpt = "-" + YearOption.KEY;
        private const string indexesCmdOpt = "-" + IndexesOption.KEY;
        private const string exportCmdOpt = "-" + ExportOption.KEY;
        private const int yearValue = 1999;
        protected readonly IList<ICommand> Commands = new List<ICommand>();
        protected IProcessor processor = null;
        protected TMService TMService { get; private set; }

        [TestInitialize]
        public void Initialize()
        {
            var logger = LoggerFactory.GetLogger(LogLevel.Error);
            TMService = new TMService
            {
                Logger = logger,
                BaseURL = BaseURL
            };

            TMCommandProcessor.ContinentFileNameFormat = ContinentFileNameFormat;
            TMCommandProcessor.CompetitionFileNameFormat = CompetitionFileNameFormat;
            TMCommandProcessor.ClubFileNameFormat = ClubFileNameFormat;
            processor = new TMCommandProcessor(
                logger,
                new Dictionary<ExportType, IExporter> {
                    { ExportType.JSON, new JsonExporter(OutputFolderPath, Level1FolderFormat) }
                },
                TMService
            );
        }

        [TestMethod, TestCategory("CMD Parsing")]
        public void TestParseCommand()
        {
            var indexes = new List<(int i1, int i2)>
            {
                (1, 1),
                (1, 2),
                (6, 1)
            };
            var args = new List<(string k, List<string> v)>
            {
                (yearCmdOpt, new List<string> { yearValue.ToString() }),
                (indexesCmdOpt, new List<string> { FormatIndexes(indexes) }),
                (exportCmdOpt, new List<string> { ExportType.JSON.ToString(), OutputFolderPath })
            };

            string cmdToParse = GenerateCmd(parseCmd, args);

            ICommand cmd = new ParseCommand(processor);
            Assert.IsTrue(cmd.Name == "parse", "Parse command's name wrongly set.");
            Assert.IsTrue(cmd.Options.Count == 3, "Parse command's Options wrong count.");

            Assert.IsTrue(cmd.CanParse(parseCmd), "Checker wrongly stated that command can not be parsed.");

            cmd.Parse(cmdToParse);
            Validate(cmdToParse, cmd, parseCmd, args, indexes);
        }

        [TestMethod, TestCategory("CMD Parsing")]
        public void TestCorrectCmdAndArgumentsParsing()
        {
            var indexes = new List<(int i1, int i2)>
            {
                (1, 1),
                (1, 2),
                (1, 3),
                (6, 1),
                (6, 2),
                (6, 3)
            };
            var args = new List<(string k, List<string> v)>
            {
                (yearCmdOpt, new List<string> { yearValue.ToString() }),
                (indexesCmdOpt, new List<string> { FormatIndexes(indexes) })
            };

            string cmdToParse = GenerateCmd(peekCmd, args);
            Command cmd = GetCommand(CommandKey.Peek);
            Assert.IsTrue(cmd.CanParse(peekCmd), "Checker wrongly stated that command can not be parsed.");

            cmd.Parse(cmdToParse);
            Validate(cmdToParse, cmd, peekCmd, args, indexes);



            //p Y:1999 1.1
            indexes = new List<(int i1, int i2)>
            {
                (1, 1)
            };
            args = new List<(string k, List<string> v)>
            {
                (yearCmdOpt, new List<string> { yearValue.ToString() }),
                (indexesCmdOpt, new List<string> { FormatIndexes(indexes) })
            };

            cmdToParse = GenerateCmd(parseCmd, args);
            cmd = GetCommand(CommandKey.Parse);
            Assert.IsTrue(cmd.CanParse(parseCmd), "Checker wrongly stated that command can not be parsed.");

            cmd.Parse(cmdToParse);
            Validate(cmdToParse, cmd, parseCmd, args, indexes);



            // P 1.1 1.3 6.2
            indexes = new List<(int i1, int i2)>
            {
                (1, 1),
                (1, 3),
                (6, 2),
            };
            args = new List<(string k, List<string> v)>
            {
                (indexesCmdOpt, new List<string> { FormatIndexes(indexes) })
            };

            cmdToParse = GenerateCmd(parseCmd, args);
            cmd = GetCommand(CommandKey.Parse);
            Assert.IsTrue(cmd.CanParse(parseCmd), "Checker wrongly stated that command can not be parsed.");

            cmd.Parse(cmdToParse);
            Validate(cmdToParse, cmd, parseCmd, args, indexes);


            // F 1.1
            indexes = new List<(int i1, int i2)>
            {
                (1, 1)
            };
            args = new List<(string k, List<string> v)>
            {
                (indexesCmdOpt, new List<string> { FormatIndexes(indexes) })
            };

            cmdToParse = GenerateCmd(peekCmd, args);
            cmd = GetCommand(CommandKey.Peek);
            Assert.IsTrue(cmd.CanParse(peekCmd), "Checker wrongly stated that command can not be parsed.");

            cmd.Parse(cmdToParse);
            Validate(cmdToParse, cmd, peekCmd, args, indexes);
        }

        [TestMethod, TestCategory("CMD Parsing")]
        public void TestBadlyFormattedButCorrectCmdAndArgumentsParsing()
        {
            var cmdType = peekCmd + "  ";
            var indexes = new List<(int i1, int i2)>
            {
                (1, 1),
                (1, 2)
            };
            var args = new List<(string k, List<string> v)>
            {
                (yearCmdOpt + "  ", new List<string> { " " + yearValue }),
                (indexesCmdOpt + " ", new List<string> { FormatIndexes(indexes) })
            };

            string cmdToParse = GenerateCmd(cmdType, args);
            Command cmd = GetCommand(CommandKey.Peek);
            Assert.IsTrue(cmd.CanParse(peekCmd), "Checker wrongly stated that command can not be parsed.");

            cmd.Parse(cmdToParse);
            Validate(cmdToParse, cmd, cmdType, args, indexes);
        }

        [TestMethod, TestCategory("CMD Parsing")]
        public void TestExceptionIsThrownIfNoIndexPassed()
        {
            var args = new List<(string k, List<string> v)>
            {
                (yearCmdOpt + "  ", new List<string> { " " + yearValue })
            };

            string cmdToParse = GenerateCmd(peekCmd, args);
            Command cmd = GetCommand(CommandKey.Peek);
            Assert.IsTrue(cmd.CanParse(peekCmd), "Checker wrongly stated that command can not be parsed.");

            var ex = Assert.ThrowsException<ArgumentException>(() => cmd.Parse(cmdToParse));
            Assert.IsTrue(ex.Message == PeekCommand.PEEK_ERROR_MSG, "Unexpected error msg");

        }

        [TestMethod, TestCategory("CMD Parsing")]
        public void TestEmptyParsing()
        {
            var cmdType = "";
            var args = new List<(string k, List<string> v)>();
            var opts = new List<(int i1, int i2)>();
            string cmdToParse = GenerateCmd(peekCmd, args);

            Command cmd = GetCommand(CommandKey.Peek);
            Assert.IsTrue(cmd.CanParse(peekCmd), "Checker wrongly stated that command can not be parsed.");

            var ex = Assert.ThrowsException<ArgumentException>(() => cmd.Parse(cmdToParse));
            Assert.IsTrue(ex.Message == PeekCommand.PEEK_ERROR_MSG, "Unexpected error msg");
        }

        [TestMethod, TestCategory("CMD Parsing")]
        public void TestFullProcess()
        {
            var indexes = new List<(int i1, int i2)> { (1, 1) };
            var args = new List<(string k, List<string> v)> { (indexesCmdOpt, new List<string> { FormatIndexes(indexes) }) };
            string cmdToParse = GenerateCmd(peekCmd, args);

            var indexes2 = new List<(int i1, int i2)> { (1, 2) };
            var args2 = new List<(string k, List<string> v)> { (indexesCmdOpt, new List<string> { FormatIndexes(indexes2) }) };
            string cmdToParse2 = GenerateCmd(peekCmd, args2);

            string cmdToParse3 = exitCmd;

            var cmds = new List<string> { cmdToParse, cmdToParse2, cmdToParse3 };

            processor.GetCommands = () => cmds;
            processor.Run();
        }

        private string FormatIndexes(List<(int i1, int i2)> opts)
        {
            return $"{string.Join(" ", opts.Select(t => string.Format("{0}.{1}", t.i1, t.i2)))}";
        }

        private string GenerateCmd(string cmdType, List<(string k, List<string> v)> args)
        {
            string cmdToParse = $"{cmdType}";
            if (args?.Count > 0)
            {
                cmdToParse += $" {string.Join(" ", args.Select(t => string.Format("{0} {1}", t.k, string.Join(" ", t.v))))}";
            }

            return cmdToParse;
        }

        private void Validate(string cmdToParse, ICommand cmd, string cmdType, List<(string k, List<string> v)> args, List<(int i1, int i2)> indexes)
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

        private void ValidateYear(ICommand cmd, List<(string k, List<string> v)> args)
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

        private void ValidateIndexes(ICommand cmd, List<(string k, List<string> v)> args, List<(int i1, int i2)> indexes)
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

        private void ValidateExport(ICommand cmd, List<(string k, List<string> v)> args)
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

        private Command GetCommand(CommandKey key)
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

        private enum CommandKey
        {
            Peek,
            Parse
        }
    }
}
