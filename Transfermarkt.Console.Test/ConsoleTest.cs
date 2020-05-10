using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Transfermarkt.Console.Test
{
    [TestClass]
    public class ConsoleTest
    {
        [TestMethod]
        public void TestArgumentsParsing()
        {
            var cmdType = "f";
            var args = new List<(string k, string v)>
            {
                ("y", "1999")
            };
            var opts = new List<(int i1, int i2)>
            {
                (1, 1),
                (1, 2),
                (1, 3),
                (6, 1),
                (6, 2),
                (6, 3)
            };
            string cmdToParse = GenerateCmd(
                cmdType,
                args,
                opts
            );
            Command cmd = Util.ParseCommand(cmdToParse);
            Validate(cmdToParse, cmd, cmdType, args, opts);


            //p Y:1999 1.1
            cmdType = "p";
            args = new List<(string k, string v)>
            {
                ("y", "1999")
            };
            opts = new List<(int i1, int i2)>
            {
                (1, 1)
            };
            cmdToParse = GenerateCmd(
                cmdType,
                args,
                opts
            );
            cmd = Util.ParseCommand(cmdToParse);
            Validate(cmdToParse, cmd, cmdType, args, opts);



            // P 1.1 1.3 6.2
            cmdType = "P";
            args = null;
            opts = new List<(int i1, int i2)>
            {
                (1, 1),
                (1, 3),
                (6, 2),
            };
            cmdToParse = GenerateCmd(
                cmdType,
                args,
                opts
            );
            cmd = Util.ParseCommand(cmdToParse);
            Validate(cmdToParse, cmd, cmdType, args, opts);


            // F 1.1
            cmdType = "F";
            args = null;
            opts = new List<(int i1, int i2)>
            {
                (1, 1)
            };
            cmdToParse = GenerateCmd(
                cmdType,
                args,
                opts
            );
            cmd = Util.ParseCommand(cmdToParse);
            Validate(cmdToParse, cmd, cmdType, args, opts);
        }

        private string GenerateCmd(string cmdType, List<(string k, string v)> args, List<(int i1, int i2)> opts)
        {
            string cmdToParse = $"{cmdType}";
            if (args?.Count > 0)
            {
                cmdToParse += $" {string.Join(" ", args.Select(t => string.Format("{0}:{1}", t.k, t.v)))}";
            }
            if (opts?.Count > 0)
            {
                cmdToParse += $" {string.Join(" ", opts.Select(t => string.Format("{0}.{1}", t.i1, t.i2)))}";
            }

            return cmdToParse;
        }

        private void Validate(string cmdToParse, Command cmd, string cmdType, List<(string k, string v)> args, List<(int i1, int i2)> opts)
        {
            CommandType? commandType = null;
            if (cmdType.ToLowerInvariant() == "f")
            {
                commandType = CommandType.F;
            }
            else if (cmdType.ToLowerInvariant() == "p")
            {
                commandType = CommandType.P;
            }

            Assert.IsTrue(cmd.CommandType == commandType, "Command Type not parsed correctly.");

            if (args?.Count > 0)
            {
                Assert.IsNotNull(cmd.ExtraArgs, $"There should exist {args.Count} extras args.");
                Assert.IsTrue(cmd.ExtraArgs.Count == args.Count, $"Number of extra args should be {args.Count} instead of {cmd.ExtraArgs.Count}.");
                for (int i = 0; i < args.Count; i++)
                {
                    ExtraCommand? extraCmd = null;
                    if (args[i].k.ToLowerInvariant() == "y")
                    {
                        extraCmd = ExtraCommand.Y;
                    }
                    Assert.IsTrue(cmd.ExtraArgs[i].Cmd == extraCmd.Value && cmd.ExtraArgs[i].Val == args[i].v, $"Extra args should be {args[i].k}:{args[i].v} instead of: {cmd.ExtraArgs[i].Cmd}:{cmd.ExtraArgs[i].Val}.");
                }
            }

            if (opts?.Count > 0)
            {
                Assert.IsNotNull(cmd.Options, $"There should exist {opts.Count} options.");
                Assert.IsTrue(cmd.Options.Count == opts.Count, $"Number of options should be {opts.Count} instead of {cmd.Options.Count}.");
                for (int i = 0; i < opts.Count; i++)
                {
                    Assert.IsTrue(cmd.Options[i].Index1 == opts[i].i1 && cmd.Options[i].Index2 == opts[i].i2, $"Extra args should be {opts[i].i1}:{opts[i].i2} instead of: {cmd.Options[i].Index1}:{cmd.Options[i].Index2}.");
                }
            }
        }
    }
}
