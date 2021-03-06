﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts.Converter;
using Transfermarkt.Core.ParseHandling.Converters;

namespace Transfermarkt.Core.Test.ParseHandling.Converters
{
    [TestClass]
    public class NationalityConverterTest
    {
        public static string SettingsFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SettingsFolderPath);
        public static string SettingsFile { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SettingsNationalityFile);

        private IDictionary<string, Type> languages = new Dictionary<string, Type>();

        public NationalityConverterTest()
        {
            languages.Add("PT", typeof(NationalityConverter));
            //languages.Add("EN", typeof(NationalityConverter));

            Assert.IsTrue(languages.Count > 0, "At least one language must exist.");
        }

        [TestMethod, TestCategory("Settings"), TestCategory("Converter"), TestCategory("Nationality Converter")]
        public void NationalityStringsAreCorrectlyTransformedIntoDomainObjects()
        {
            INationalityConverter converter;

            foreach (var language in languages)
            {
                converter = (INationalityConverter)Activator.CreateInstance(language.Value);

                string json = File.ReadAllText($@"{SettingsFolderPath}\{language.Key}\{SettingsFile}");

                var definition = new { Language = default(string), Set = new[] { new { Name = default(string), DO = default(string) } } };
                var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
                foreach (var item in deserializedJSON.Set)
                {
                    NationalityValue retValue = (NationalityValue)converter.Convert(item.Name);

                    Assert.IsTrue(retValue.Value.HasValue, $"The Nationality string \"{item.Name}\" didn't translate into a Nationality type domain object.");
                }
            }
        }

        [TestMethod, TestCategory("Settings"), TestCategory("Converter"), TestCategory("Nationality Converter")]
        public void IncorrectNationalityStringIsNotTransformedIntoDomainObjects()
        {
            INationalityConverter converter = new NationalityConverter();
            NationalityValue retValue = (NationalityValue)converter.Convert("Stupid name that doesn't exist in the file");
            Assert.IsFalse(retValue.Value.HasValue, $"Value should have been false because the supplied name doesn't exist.");

            retValue = (NationalityValue)converter.Convert(null);
            Assert.IsFalse(retValue.Value.HasValue, $"Value should have been null because false was supplied as the value to translate.");
        }
    }
}
