using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts.Converters;

namespace Transfermarkt.Core.Converters.Foot
{
    [TestClass]
    public class FootConverterTest
    {
        public static string SettingsFolderPath { get; } = ConfigurationManager.AppSettings["SettingsFolderPath"].ToString();
        public static string SettingsFootFile { get; } = ConfigurationManager.AppSettings["SettingsFootFile"].ToString();

        private IDictionary<string, Type> languages = new Dictionary<string, Type>();

        public FootConverterTest()
        {
            languages.Add("PT", typeof(PTFootConverter));
            languages.Add("EN", typeof(ENFootConverter));

            Assert.IsTrue(languages.Count > 0, "At least one language must exist.");
        }

        [TestMethod, TestCategory("Settings")]
        public void FootStringsAreCorrectlyTransformedIntoDomainObjects()
        {
            IFootConverter converter;

            foreach (var language in languages)
            {
                converter = (IFootConverter)Activator.CreateInstance(language.Value);

                string json = File.ReadAllText($@"{SettingsFolderPath}\{language.Key}\{SettingsFootFile}");

                var definition = new { Language = default(string), Set = new[] { new { Name = default(string), DO = default(string) } } };
                var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
                foreach (var item in deserializedJSON.Set)
                {
                    Actors.Foot? retValue = converter.Convert(item.Name);

                    Assert.IsNotNull(retValue, $"The Foot string \"{item.Name}\" didn't translate into a Foot type domain object.");
                }
            }
        }

        [TestMethod, TestCategory("Settings")]
        public void IncorrectFootStringIsNotTransformedIntoDomainObjects()
        {
            IFootConverter converter = new PTFootConverter();
            Actors.Foot? retValue = converter.Convert("Stupid name that doesn't exist in the file");
            Assert.IsNull(retValue, $"Value should have been null because the supplied name doesn't exist.");

            retValue = converter.Convert(null);
            Assert.IsNull(retValue, $"Value should have been null because null was supplied as the value to translate.");
        }
    }
}
