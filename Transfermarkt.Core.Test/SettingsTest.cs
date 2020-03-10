using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Test
{
    [TestClass]
    public class SettingsTest
    {
        [TestMethod, TestCategory("Core"), TestCategory("Nomenclatures")]
        public void ConfigKeysHaveCorrespondentEnumEntry()
        {
            FieldInfo[] fi = typeof(Keys.Config).GetFields();
            foreach (string key in ConfigurationManager.AppSettings)
            {
                Assert.IsTrue(fi.Any(f => f.Name == key), $"Key does not exist in the {nameof(Keys.Config)}");
            }
        }
    }
}
