﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.Contracts.Converters;
using Transfermarkt.Core.Converters;

namespace Transfermarkt.Core.Test.Converters
{
    [TestClass]
    public class PositionConverterTest
    {
        private static IConfigurationManager config = new ConfigManager();

        public static string SettingsFolderPath { get; } = config.GetAppSetting("SettingsFolderPath");
        public static string SettingsFile { get; } = config.GetAppSetting("SettingsPositionFile");

        private IDictionary<string, Type> languages = new Dictionary<string, Type>();

        public PositionConverterTest()
        {
            languages.Add("PT", typeof(PositionConverter));
            //languages.Add("EN", typeof(PositionConverter));

            Assert.IsTrue(languages.Count > 0, "At least one language must exist.");
        }

        [TestMethod, TestCategory("Settings"), TestCategory("Converter"), TestCategory("Position Converter")]
        public void SettingsPositionIsCorrectlyRead()
        {
            IPositionConverter converter;

            foreach (var language in languages)
            {
                converter = (IPositionConverter)Activator.CreateInstance(language.Value);
            }
        }

        [TestMethod, TestCategory("Settings"), TestCategory("Converter"), TestCategory("Position Converter")]
        public void PositionStringsAreCorrectlyTransformedIntoDomainObjects()
        {
            IPositionConverter converter;

            foreach (var language in languages)
            {
                converter = (IPositionConverter)Activator.CreateInstance(language.Value);

                string json = File.ReadAllText($@"{SettingsFolderPath}\{language.Key}\{SettingsFile}");

                var definition = new { Language = default(string), Set = new[] { new { Name = default(string), DO = default(string) } } };
                var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
                foreach (var item in deserializedJSON.Set)
                {
                    Actors.Position? retValue = converter.Convert(item.Name);

                    Assert.IsTrue(retValue.HasValue, $"The Position string \"{item.Name}\" didn't translate into a Position type domain object.");
                }
            }
        }

        [TestMethod, TestCategory("Settings"), TestCategory("Converter"), TestCategory("Position Converter")]
        public void IncorrectPositionStringIsNotTransformedIntoDomainObjects()
        {
            IPositionConverter converter = new PositionConverter();
            Actors.Position? retValue = converter.Convert("Stupid name that doesn't exist in the file");
            Assert.IsFalse(retValue.HasValue, $"Value should have been null because the supplied name doesn't exist.");

            retValue = converter.Convert(null);
            Assert.IsFalse(retValue.HasValue, $"Value should have been null because null was supplied as the value to translate.");
        }
    }
}