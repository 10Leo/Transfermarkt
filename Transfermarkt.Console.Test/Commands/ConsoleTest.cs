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
                (Util.yearCmdOpt, new List<string> { yearValue.ToString() }),
                (Util.indexesCmdOpt, new List<string> { Util.FormatIndexes(indexes) })
            };

            string cmdToParse = Util.GenerateCmd(Util.peekCmd, args);
            Command cmd = Util.GetCommand(CommandKey.Peek, TMService);
            Assert.IsTrue(cmd.CanParse(Util.peekCmd), "Checker wrongly stated that command can not be parsed.");

            cmd.Parse(cmdToParse);
            Util.Validate(cmdToParse, cmd, Util.peekCmd, args, indexes);



            //p Y:1999 1.1
            indexes = new List<(int i1, int i2)>
            {
                (1, 1)
            };
            args = new List<(string k, List<string> v)>
            {
                (Util.yearCmdOpt, new List<string> { yearValue.ToString() }),
                (Util.indexesCmdOpt, new List<string> { Util.FormatIndexes(indexes) })
            };

            cmdToParse = Util.GenerateCmd(Util.parseCmd, args);
            cmd = Util.GetCommand(CommandKey.Parse, TMService);
            Assert.IsTrue(cmd.CanParse(Util.parseCmd), "Checker wrongly stated that command can not be parsed.");

            cmd.Parse(cmdToParse);
            Util.Validate(cmdToParse, cmd, Util.parseCmd, args, indexes);



            // P 1.1 1.3 6.2
            indexes = new List<(int i1, int i2)>
            {
                (1, 1),
                (1, 3),
                (6, 2),
            };
            args = new List<(string k, List<string> v)>
            {
                (Util.indexesCmdOpt, new List<string> { Util.FormatIndexes(indexes) })
            };

            cmdToParse = Util.GenerateCmd(Util.parseCmd, args);
            cmd = Util.GetCommand(CommandKey.Parse, TMService);
            Assert.IsTrue(cmd.CanParse(Util.parseCmd), "Checker wrongly stated that command can not be parsed.");

            cmd.Parse(cmdToParse);
            Util.Validate(cmdToParse, cmd, Util.parseCmd, args, indexes);


            // F 1.1
            indexes = new List<(int i1, int i2)>
            {
                (1, 1)
            };
            args = new List<(string k, List<string> v)>
            {
                (Util.indexesCmdOpt, new List<string> { Util.FormatIndexes(indexes) })
            };

            cmdToParse = Util.GenerateCmd(Util.peekCmd, args);
            cmd = Util.GetCommand(CommandKey.Peek, TMService);
            Assert.IsTrue(cmd.CanParse(Util.peekCmd), "Checker wrongly stated that command can not be parsed.");

            cmd.Parse(cmdToParse);
            Util.Validate(cmdToParse, cmd, Util.peekCmd, args, indexes);
        }

        [TestMethod, TestCategory("CMD Parsing"), Priority(2)]
        public void TestFullProcess()
        {
            var indexes = new List<(int i1, int i2)> { (1, 1) };
            var args = new List<(string k, List<string> v)> { (Util.indexesCmdOpt, new List<string> { Util.FormatIndexes(indexes) }) };
            string cmdToParse = Util.GenerateCmd(Util.peekCmd, args);

            var indexes2 = new List<(int i1, int i2)> { (1, 2) };
            var args2 = new List<(string k, List<string> v)> { (Util.indexesCmdOpt, new List<string> { Util.FormatIndexes(indexes2) }) };
            string cmdToParse2 = Util.GenerateCmd(Util.peekCmd, args2);

            string cmdToParse3 = Util.exitCmd;

            var cmds = new List<string> { cmdToParse, cmdToParse2, cmdToParse3 };

            processor.GetCommands = () => cmds;
            processor.Run();
        }
    }
}
