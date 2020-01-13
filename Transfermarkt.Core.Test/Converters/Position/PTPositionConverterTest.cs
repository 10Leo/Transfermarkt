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
    public class PTPositionConverterTest
    {
        public static string SettingsPTFolderPath { get; } = ConfigurationManager.AppSettings["SettingsPTFolderPath"].ToString();
        public static string SettingsPTPositionsFile { get; } = ConfigurationManager.AppSettings["SettingsPTPositionsFile"].ToString();

        [TestMethod, TestCategory("Settings")]
        public void SettingsPositionIsCorrectlyRead()
        {
            IPositionConverter converter = new PTPositionConverter();
        }

        [TestMethod, TestCategory("Settings")]
        public void PositionStringsAreCorrectlyTransformedToDomainObjects()
        {
            IPositionConverter converter = new PTPositionConverter();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            string json = File.ReadAllText($@"{SettingsPTFolderPath}\{SettingsPTPositionsFile}");

            var definition = new { Language = default(string), Positions = new[] { new { Name = default(string), DO = default(string) } } };
            var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            foreach (var item in deserializedJSON.Positions)
            {
                Actors.Position? retValue = converter.Convert(item.Name);

                Assert.IsNotNull(retValue, $"The Position string \"{item.Name}\" didn't translate into a Position type domain object.");
            }
        }

        [TestMethod, TestCategory("Settings")]
        public void IncorrectPositionStringIsNotTransformedToDomainObjects()
        {
            IPositionConverter converter = new PTPositionConverter();
            Actors.Position? retValue = converter.Convert("Stupid name that doesn't exist in the file");
            Assert.IsNull(retValue, $"Value should have been null because the supplied name doesn't exist.");

            retValue = converter.Convert(null);
            Assert.IsNull(retValue, $"Value should have been null because null was supplied as the value to translate.");
        }
    }
}
