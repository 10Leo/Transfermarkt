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
    public class ParseCommandTest
    {
        private const int yearValue = 1999;
        protected IProcessor processor = null;
        protected TMService TMService { get; private set; }

        [TestInitialize]
        public void Initialize()
        {
            var logger = LoggerFactory.GetLogger(LogLevel.Error);
            TMService = new TMService
            {
                Logger = logger,
                BaseURL = Util.BaseURL
            };

            TMCommandProcessor.ContinentFileNameFormat = Util.ContinentFileNameFormat;
            TMCommandProcessor.CompetitionFileNameFormat = Util.CompetitionFileNameFormat;
            TMCommandProcessor.ClubFileNameFormat = Util.ClubFileNameFormat;
            processor = new TMCommandProcessor(
                logger,
                new Dictionary<ExportType, IExporter> {
                    { ExportType.JSON, new JsonExporter(Util.OutputFolderPath, Util.Level1FolderFormat) }
                },
                TMService
            );
        }

        [TestMethod, TestCategory("CMD Parsing"), Priority(1)]
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
                (Util.yearCmdOpt, new List<string> { yearValue.ToString() }),
                (Util.indexesCmdOpt, new List<string> { Util.FormatIndexes(indexes) }),
                //(exportCmdOpt, new List<string> { ExportType.JSON.ToString(), OutputFolderPath })
            };

            string cmdToParse = Util.GenerateCmd(Util.parseCmd, args);

            ICommand cmd = new ParseCommand(processor);
            Assert.IsTrue(cmd.Name == "parse", "Parse command's name wrongly set.");
            Assert.IsTrue(cmd.Options.Count == 3, "Parse command's Options wrong count.");

            Assert.IsTrue(cmd.CanParse(Util.parseCmd), "Checker wrongly stated that command can not be parsed.");

            cmd.Parse(cmdToParse);
            Util.Validate(cmdToParse, cmd, Util.parseCmd, args, indexes);
        }

        [TestMethod, TestCategory("CMD Parsing"), Priority(1)]
        public void TestBadlyFormattedButCorrectParseCmdAndArgumentsParsing()
        {
            var cmdType = Util.parseCmd + "  ";
            var indexes = new List<(int i1, int i2)>
            {
                (1, 1),
                (1, 2)
            };
            var args = new List<(string k, List<string> v)>
            {
                (Util.yearCmdOpt + "  ", new List<string> { " " + yearValue }),
                (Util.indexesCmdOpt + " ", new List<string> { Util.FormatIndexes(indexes) })
            };

            string cmdToParse = Util.GenerateCmd(cmdType, args);
            Command cmd = Util.GetCommand(CommandKey.Parse, TMService);
            Assert.IsTrue(cmd.CanParse(Util.parseCmd), "Checker wrongly stated that command can not be parsed.");

            cmd.Parse(cmdToParse);
            Util.Validate(cmdToParse, cmd, cmdType, args, indexes);
        }

        [TestMethod, TestCategory("CMD Parsing"), Priority(1)]
        public void TestExceptionIsThrownIfNoIndexPassed()
        {
            var args = new List<(string k, List<string> v)>
            {
                (Util.yearCmdOpt + "  ", new List<string> { " " + yearValue })
            };

            string cmdToParse = Util.GenerateCmd(Util.parseCmd, args);
            Command cmd = Util.GetCommand(CommandKey.Parse, TMService);
            Assert.IsTrue(cmd.CanParse(Util.parseCmd), "Checker wrongly stated that command can not be parsed.");

            var ex = Assert.ThrowsException<Exception>(() => cmd.Parse(cmdToParse));
            Assert.IsTrue(ex.Message == string.Format(LJMB.Command.Option.Exceptions.OPTION_NOT_FOUND_ERROR_MSG, new IndexesOption().Name), "Unexpected error msg");
        }

        [TestMethod, TestCategory("CMD Parsing"), Priority(1)]
        public void TestParseCMDNoOptionsParsing()
        {
            var args = new List<(string k, List<string> v)>();
            var opts = new List<(int i1, int i2)>();
            string cmdToParse = Util.GenerateCmd(Util.parseCmd, args);

            Command cmd = Util.GetCommand(CommandKey.Parse, TMService);
            Assert.IsTrue(cmd.CanParse(Util.parseCmd), "Checker wrongly stated that command can not be parsed.");

            var ex = Assert.ThrowsException<Exception>(() => cmd.Parse(cmdToParse));
            Assert.IsTrue(ex.Message == string.Format(Option.Exceptions.OPTION_NOT_FOUND_ERROR_MSG, new IndexesOption().Name), "Unexpected error msg");
        }
    }
}
