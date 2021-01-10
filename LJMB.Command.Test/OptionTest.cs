using System;
using System.Collections.Generic;
using LJMB.Command.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LJMB.Command.Test
{
    [TestClass]
    public class OptionTest
    {
        private readonly List<IOption> options = new List<IOption>();
        
        [TestInitialize]
        public void Initialize()
        {
            options.Add(new MockOption());
        }


    }
}
