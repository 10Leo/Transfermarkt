using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts.Converter;
using Transfermarkt.Core.ParseHandling.Converters;

namespace Transfermarkt.Core.Test.ParseHandling.Converters
{
    [TestClass]
    public class FootConverterTest
    {
        private static IConfigurationManager config = new ConfigManager();

        public static string SettingsFolderPath { get; } = config.GetAppSetting("SettingsFolderPath");
        public static string SettingsFile { get; } = config.GetAppSetting("SettingsFootFile");

        private IDictionary<string, Type> languages = new Dictionary<string, Type>();

        public FootConverterTest()
        {
            languages.Add("PT", typeof(FootConverter));
            //languages.Add("EN", typeof(FootConverter));

            Assert.IsTrue(languages.Count > 0, "At least one language must exist.");
        }

        [TestMethod, TestCategory("Settings"), TestCategory("Converter"), TestCategory("Foot Converter")]
        public void FootStringsAreCorrectlyTransformedIntoDomainObjects()
        {
            IFootConverter converter;

            foreach (var language in languages)
            {
                converter = (IFootConverter)Activator.CreateInstance(language.Value);

                string json = File.ReadAllText($@"{SettingsFolderPath}\{language.Key}\{SettingsFile}");

                var definition = new { Language = default(string), Set = new[] { new { Name = default(string), DO = default(string) } } };
                var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
                foreach (var item in deserializedJSON.Set)
                {
                    Actors.Foot? retValue = converter.Convert(item.Name);

                    Assert.IsTrue(retValue.HasValue, $"The Foot string \"{item.Name}\" didn't translate into a Foot type domain object.");
                }
            }
        }

        [TestMethod, TestCategory("Settings"), TestCategory("Converter"), TestCategory("Foot Converter")]
        public void IncorrectFootStringIsNotTransformedIntoDomainObjects()
        {
            IFootConverter converter = new FootConverter();
            Actors.Foot? retValue = converter.Convert("Stupid name that doesn't exist in the file");
            Assert.IsFalse(retValue.HasValue, $"Value should have been null because the supplied name doesn't exist.");

            retValue = converter.Convert(null);
            Assert.IsFalse(retValue.HasValue, $"Value should have been null because null was supplied as the value to translate.");
        }
    }
}
