using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Transfermarkt.Core.Contracts.Converters;
using Transfermarkt.Core.Converters;

namespace Transfermarkt.Core.Test.Converters.Position
{
    [TestClass]
    public class PositionConverterTest
    {
        public static string SettingsFolderPath { get; } = ConfigurationManager.AppSettings["SettingsFolderPath"].ToString();
        public static string SettingsPositionsFile { get; } = ConfigurationManager.AppSettings["SettingsPositionsFile"].ToString();

        private IDictionary<string, Type> languages = new Dictionary<string, Type>();

        public PositionConverterTest()
        {
            languages.Add("PT", typeof(PTPositionConverter));
            languages.Add("EN", typeof(ENPositionConverter));

            Assert.IsTrue(languages.Count > 0, "At least one language must exist.");
        }

        [TestMethod, TestCategory("Settings")]
        public void SettingsPositionIsCorrectlyRead()
        {
            IPositionConverter converter;

            foreach (var language in languages)
            {
                converter = (IPositionConverter)Activator.CreateInstance(language.Value);
            }
        }

        [TestMethod, TestCategory("Settings")]
        public void PositionStringsAreCorrectlyTransformedIntoDomainObjects()
        {
            IPositionConverter converter;

            foreach (var language in languages)
            {
                converter = (IPositionConverter)Activator.CreateInstance(language.Value);

                string json = File.ReadAllText($@"{SettingsFolderPath}\{language.Key}\{SettingsPositionsFile}");

                var definition = new { Language = default(string), Set = new[] { new { Name = default(string), DO = default(string) } } };
                var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
                foreach (var item in deserializedJSON.Set)
                {
                    Actors.Position? retValue = converter.Convert(item.Name);

                    Assert.IsNotNull(retValue, $"The Position string \"{item.Name}\" didn't translate into a Position type domain object.");
                }
            }
        }

        [TestMethod, TestCategory("Settings")]
        public void IncorrectPositionStringIsNotTransformedIntoDomainObjects()
        {
            IPositionConverter converter = new PTPositionConverter();
            Actors.Position? retValue = converter.Convert("Stupid name that doesn't exist in the file");
            Assert.IsNull(retValue, $"Value should have been null because the supplied name doesn't exist.");

            retValue = converter.Convert(null);
            Assert.IsNull(retValue, $"Value should have been null because null was supplied as the value to translate.");
        }
    }
}
