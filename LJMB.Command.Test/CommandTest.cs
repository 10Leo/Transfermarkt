using System;
using System.Collections.Generic;
using LJMB.Command.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LJMB.Command.Test
{
    [TestClass]
    public class CommandTest
    {
        private IProcessor processor = null;
        private readonly List<ICommand> commands = new List<ICommand>();
        
        [TestInitialize]
        public void Initialize()
        {
            processor = new MockProcessor();

            commands.Add(new ExitCommand(processor));
            commands.Add(new ListCommand(processor));
            foreach (var cmd in commands)
            {
                processor.RegisterCommand(cmd);
            }
        }

        [TestMethod, TestCategory("CommandProcessor")]
        public void TestCommandCount()
        {
            Assert.IsTrue(processor.Commands.Count == commands.Count, $"{commands.Count} commands expected, {processor.Commands.Count} found.");
            for (int i = 0; i < processor.Commands.Count; i++)
            {
                // In time, if more commands are added to the project, this might not be right, as the ISet where the commands
                // are stored might be ordered differently than the List.
                Assert.IsTrue(processor.Commands[i] == commands[i]);
            }
        }

        [TestMethod, TestCategory("CommandProcessor")]
        public void TestRepeatedCommand()
        {
            var ex = Assert.ThrowsException<Exception>(() => processor.RegisterCommand(new ExitCommand(processor)));
            Assert.IsTrue(ex.Message == Processor.REPEATED_COMMAND_ERROR_MSG, "Unexpected error msg");
        }

        [TestMethod, TestCategory("CommandProcessor")]
        public void TestProcessor()
        {
            Assert.IsTrue(ListCommand.KEY == Util.ListCmd, $"{ListCommand.NAME} has an unexpected key {ListCommand.KEY}");
            Assert.IsTrue(ExitCommand.KEY == Util.ExitCmd, $"{ExitCommand.NAME} has an unexpected key {ExitCommand.KEY}");

            string cmdToParse = ListCommand.KEY;
            string cmdToParse2 = ExitCommand.KEY;

            var cmds = new List<string> { cmdToParse, cmdToParse2 };

            processor.GetCommands = () => cmds;
            processor.Run();

            Assert.IsTrue(processor.Exit, "Exit should be set to true as the Exit command was invoked.");
        }
    }
}
