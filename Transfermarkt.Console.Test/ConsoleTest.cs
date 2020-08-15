using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Transfermarkt.Console.Test
{
    [TestClass]
    public class ConsoleTest
    {
        //[TestMethod, TestCategory("CMD Parsing")]
        //public void TestCorrectCmdAndArgumentsParsing()
        //{
        //    var cmdType = "f";
        //    var opts = new List<(int i1, int i2)>
        //    {
        //        (1, 1),
        //        (1, 2),
        //        (1, 3),
        //        (6, 1),
        //        (6, 2),
        //        (6, 3)
        //    };
        //    var args = new List<(string k, string v)>
        //    {
        //        ("-y", "1999"),
        //        ("-o", FormatIndexes(opts))
        //    };
            
        //    string cmdToParse = GenerateCmd(cmdType, args);
        //    Command cmd = CommandUtil.ParseExecuteCommand(cmdToParse);
        //    Validate(cmdToParse, cmd, cmdType, args, opts);


        //    //p Y:1999 1.1
        //    cmdType = "p";
        //    opts = new List<(int i1, int i2)>
        //    {
        //        (1, 1)
        //    };
        //    args = new List<(string k, string v)>
        //    {
        //        ("-y", "1999"),
        //        ("-o", FormatIndexes(opts))
        //    };
            
        //    cmdToParse = GenerateCmd(cmdType, args);
        //    cmd = CommandUtil.ParseExecuteCommand(cmdToParse);
        //    Validate(cmdToParse, cmd, cmdType, args, opts);



        //    // P 1.1 1.3 6.2
        //    cmdType = "P";
        //    opts = new List<(int i1, int i2)>
        //    {
        //        (1, 1),
        //        (1, 3),
        //        (6, 2),
        //    };
        //    args = new List<(string k, string v)>
        //    {
        //        ("-o", FormatIndexes(opts))
        //    };

        //    cmdToParse = GenerateCmd(cmdType, args);
        //    cmd = CommandUtil.ParseExecuteCommand(cmdToParse);
        //    Validate(cmdToParse, cmd, cmdType, args, opts);


        //    // F 1.1
        //    cmdType = "F";
        //    opts = new List<(int i1, int i2)>
        //    {
        //        (1, 1)
        //    };
        //    args = new List<(string k, string v)>
        //    {
        //        ("-o", FormatIndexes(opts))
        //    };

        //    cmdToParse = GenerateCmd(cmdType, args);
        //    cmd = CommandUtil.ParseExecuteCommand(cmdToParse);
        //    Validate(cmdToParse, cmd, cmdType, args, opts);
        //}

        //[TestMethod, TestCategory("CMD Parsing")]
        //public void TestBadlyFormattedButCorrectCmdAndArgumentsParsing()
        //{
        //    var cmdType = "f  ";
        //    var opts = new List<(int i1, int i2)>
        //    {
        //        (1, 1),
        //        (1, 2)
        //    };
        //    var args = new List<(string k, string v)>
        //    {
        //        ("-y  ", " 1999"),
        //        ("-o ", FormatIndexes(opts))
        //    };
            
        //    string cmdToParse = GenerateCmd(cmdType, args);
        //    Command cmd = CommandUtil.ParseExecuteCommand(cmdToParse);
        //    Validate(cmdToParse, cmd, cmdType, args, opts);
        //}

        //[TestMethod, TestCategory("CMD Parsing")]
        //public void TestEmptyParsing()
        //{
        //    var cmdType = "";
        //    var args = new List<(string k, string v)>();
        //    var opts = new List<(int i1, int i2)>();
        //    string cmdToParse = GenerateCmd(cmdType, args);

        //    try
        //    {
        //        Command cmd = CommandUtil.ParseExecuteCommand(cmdToParse);
        //        Validate(cmdToParse, cmd, cmdType, args, opts);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.IsTrue(ex.Message == CommandUtil.ErrorMsg.ERROR_MSG_CMD);
        //    }


        //    cmdType = "   ";
        //    args = new List<(string k, string v)>();
        //    opts = new List<(int i1, int i2)>();
        //    cmdToParse = GenerateCmd(cmdType, args);

        //    try
        //    {
        //        Command cmd = CommandUtil.ParseExecuteCommand(cmdToParse);
        //        Validate(cmdToParse, cmd, cmdType, args, opts);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.IsTrue(ex.Message == CommandUtil.ErrorMsg.ERROR_MSG_CMD);
        //    }
        //}

        //[TestMethod, TestCategory("CMD Parsing")]
        //public void TestEmptyCmdParsing()
        //{
        //    var cmdType = "";
        //    var opts = new List<(int i1, int i2)>
        //    {
        //        (1, 1),
        //        (1, 2)
        //    };
        //    var args = new List<(string k, string v)>
        //    {
        //        ("-y", "1999"),
        //        ("-o", FormatIndexes(opts))
        //    };
            
        //    string cmdToParse = GenerateCmd(cmdType, args);

        //    try
        //    {
        //        Command cmd = CommandUtil.ParseExecuteCommand(cmdToParse);
        //        Validate(cmdToParse, cmd, cmdType, args, opts);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.IsTrue(ex.Message == CommandUtil.ErrorMsg.ERROR_MSG_CMD);
        //    }
        //}

        //[TestMethod, TestCategory("CMD Parsing")]
        //public void TestErroneousgCmdParsing()
        //{
        //    var cmdType = "fet";
        //    var opts = new List<(int i1, int i2)>
        //    {
        //        (1, 1),
        //        (1, 2)
        //    };
        //    var args = new List<(string k, string v)>
        //    {
        //        ("-y", "1999"),
        //        ("-o", FormatIndexes(opts))
        //    };
            
        //    string cmdToParse = GenerateCmd(cmdType, args);

        //    try
        //    {
        //        Command cmd = CommandUtil.ParseExecuteCommand(cmdToParse);
        //        Validate(cmdToParse, cmd, cmdType, args, opts);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.IsTrue(ex.Message == CommandUtil.ErrorMsg.ERROR_MSG_CMD);
        //    }
        //}

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

        //private void Validate(string cmdToParse, Command cmd, string cmdType, List<(string k, string v)> args, List<(int i1, int i2)> opts)
        //{
        //    Action? action = null;
        //    if (cmdType.Trim().ToLowerInvariant() == "f")
        //    {
        //        action = Action.F;
        //    }
        //    else if (cmdType.Trim().ToLowerInvariant() == "p")
        //    {
        //        action = Action.P;
        //    }

        //    Assert.IsTrue(cmd.Action == action, "Command Type not parsed correctly.");

        //    if (args?.Count > 0)
        //    {
        //        Assert.IsNotNull(cmd.Parameters, $"There should exist {args.Count} extras args.");
        //        Assert.IsTrue(cmd.Parameters.Count == args.Count, $"Number of extra args should be {args.Count} instead of {cmd.Parameters.Count}.");
        //        for (int i = 0; i < args.Count; i++)
        //        {
        //            OptionName? arg = null;
        //            if (args[i].k.Trim().ToLowerInvariant() == "-y")
        //            {
        //                arg = OptionName.Y;

        //                var stringArgType = (StringParameterValue)cmd.Parameters[i].Val;
        //                Assert.IsTrue(
        //                    stringArgType.Value == args[i].v.Trim().ToLowerInvariant(),
        //                    $"Extra args should be {args[i].k.Trim().ToLowerInvariant()}:{args[i].v.Trim().ToLowerInvariant()} instead of: {cmd.Parameters[i].Cmd}:{stringArgType.Value}."
        //                );
        //            }
        //            else if (args[i].k.Trim().ToLowerInvariant() == "-o")
        //            {
        //                arg = OptionName.I;

        //                if (cmd.Parameters[i].Val is Index2ParameterValue)
        //                {
        //                    var indexArgType = (Index2ParameterValue)cmd.Parameters[i].Val;

        //                    Assert.IsNotNull(indexArgType, $"There should exist {opts.Count} options.");
        //                    //Assert.IsTrue(
        //                    //    indexArgType.Indexes.Count == opts.Count,
        //                    //    $"Number of options should be {opts.Count} instead of {indexArgType.Indexes.Count}."
        //                    //);
        //                    //for (int j = 0; j < opts.Count; j++)
        //                    //{
        //                    //    Assert.IsTrue(indexArgType.Indexes[j].Index1 == opts[j].i1 && indexArgType.Indexes[j].Index2 == opts[j].i2, $"Extra args should be {opts[j].i1}:{opts[j].i2} instead of: {indexArgType.Indexes[j].Index1}:{indexArgType.Indexes[j].Index2}.");
        //                    //}
        //                }
        //            }
        //            Assert.IsTrue(cmd.Parameters[i].Cmd == arg.Value, $"Extra args should be {args[i].k}:{args[i].v} instead of: {cmd.Parameters[i].Cmd}:{cmd.Parameters[i].Val}.");
        //        }
        //    }
        //}
    }
}
