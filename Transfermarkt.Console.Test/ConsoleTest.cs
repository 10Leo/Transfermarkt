using System;
using System.Collections.Generic;
using System.Linq;
using LJMB.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transfermarkt.Console.Arguments;
using Transfermarkt.Console.Options;

namespace Transfermarkt.Console.Test
{
    [TestClass]
    public class ConsoleTest
    {
        private const string peekCmd = "f";
        private const string parseCmd = "p";
        private const string exitCmd = "e";
        private const string yearCmdOpt = "-y";
        private const string indexesCmdOpt = "-i";
        private const int yearValue = 1999;

        protected readonly IList<ICommand> Commands = new List<ICommand>();
        protected IContext context = null;

        [TestInitialize]
        public void Initialize()
        {
            context = new TMContext();
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
            var args = new List<(string k, string v)>
            {
                (yearCmdOpt, yearValue.ToString()),
                (indexesCmdOpt, FormatIndexes(indexes))
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
            args = new List<(string k, string v)>
            {
                (yearCmdOpt, yearValue.ToString()),
                (indexesCmdOpt, FormatIndexes(indexes))
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
            args = new List<(string k, string v)>
            {
                (indexesCmdOpt, FormatIndexes(indexes))
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
            args = new List<(string k, string v)>
            {
                (indexesCmdOpt, FormatIndexes(indexes))
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
            var args = new List<(string k, string v)>
            {
                (yearCmdOpt + "  ", " " + yearValue),
                (indexesCmdOpt + " ", FormatIndexes(indexes))
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
            var args = new List<(string k, string v)>
            {
                (yearCmdOpt + "  ", " " + yearValue)
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
            var args = new List<(string k, string v)>();
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
            var args = new List<(string k, string v)> { (indexesCmdOpt, FormatIndexes(indexes)) };
            string cmdToParse = GenerateCmd(peekCmd, args);

            var indexes2 = new List<(int i1, int i2)> { (1, 2) };
            var args2 = new List<(string k, string v)> { (indexesCmdOpt, FormatIndexes(indexes2)) };
            string cmdToParse2 = GenerateCmd(peekCmd, args2);

            string cmdToParse3 = exitCmd;

            var cmds = new List<string> { cmdToParse, cmdToParse2, cmdToParse3 };

            context.GetCommands = () => cmds;
            context.Run();
        }

        private string FormatIndexes(List<(int i1, int i2)> opts)
        {
            return $"{string.Join(" ", opts.Select(t => string.Format("{0}.{1}", t.i1, t.i2)))}";
        }

        private string GenerateCmd(string cmdType, List<(string k, string v)> args)
        {
            string cmdToParse = $"{cmdType}";
            if (args?.Count > 0)
            {
                cmdToParse += $" {string.Join(" ", args.Select(t => string.Format("{0} {1}", t.k, t.v)))}";
            }

            return cmdToParse;
        }

        private void Validate(string cmdToParse, Command cmd, string cmdType, List<(string k, string v)> args, List<(int i1, int i2)> indexes)
        {
            if (args?.Count > 0)
            {
                //Assert.IsNotNull(cmd.Options, $"There should exist {args.Count} extras args.");
                //Assert.IsTrue(cmd.Options.Count == args.Count, $"Number of extra args should be {args.Count} instead of {cmd.Options.Count}.");

                ValidateYear(cmd, args);
                ValidateIndexes(cmd, args, indexes);
            }
        }

        private void ValidateYear(Command cmd, List<(string k, string v)> args)
        {
            (string k, string v) yy = args.FirstOrDefault(a => a.k.Trim().ToLowerInvariant() == yearCmdOpt);
            if (yy.k != null)
            {
                // args.Count(a => a.k == yearCmdOpt) == 1
                var yearOpt = cmd[YearOption.Key] as YearOption;

                Assert.IsNotNull(yearOpt, $"{YearOption.Key} option not found");
                Assert.IsTrue((yearOpt.Args.FirstOrDefault() as StringArgument).Value == yy.v.Trim().ToLowerInvariant());
            }
        }

        private void ValidateIndexes(Command cmd, List<(string k, string v)> args, List<(int i1, int i2)> indexes)
        {
            (string k, string v) ii = args.FirstOrDefault(a => a.k.Trim().ToLowerInvariant() == indexesCmdOpt);
            if (ii.k != null)
            {
                var indexesOpt = cmd[IndexesOption.Key] as IndexesOption;

                Assert.IsNotNull(indexesOpt, $"{YearOption.Key} option not found");

                for (int j = 0; j < indexes.Count; j++)
                {
                    (int i1, int i2) ij = indexes[j];

                    var ar = indexesOpt.Args.ElementAt(j) as Index2Argument;
                    Assert.IsTrue(ar.Index1 == ij.i1 && ar.Index2 == ij.i2);
                }
            }
        }

        private Command GetCommand(CommandKey key)
        {
            switch (key)
            {
                case CommandKey.Peek:
                    return new PeekCommand(new TMContext());
                case CommandKey.Parse:
                    return new ParseCommand(new TMContext());
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
