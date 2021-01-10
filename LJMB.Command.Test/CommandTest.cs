using System;
using System.Collections.Generic;
using LJMB.Command.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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
        }

        [TestMethod, TestCategory("Commands"), Priority(1)]
        public void TestCommands()
        {
            var r = new Random();

            foreach (ICommand cmd in commands)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(cmd.Name), $"Name property is empty");
                Assert.IsTrue(!string.IsNullOrEmpty(cmd.Usage), $"Usage property is empty");

                int index = r.Next(0, cmd.AllowedAlias.Count - 1);
                Assert.IsTrue(cmd.CanParse(cmd.AllowedAlias.ElementAt(index)), $"Usage property is empty");
            }
        }
    }
}
