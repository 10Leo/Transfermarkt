using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.Contracts.Converters;
using Transfermarkt.Core.Converters;

namespace Transfermarkt.Core.Test.Converters
{
    [TestClass]
    public class NationalityConverterTest
    {
        private static IConfigurationManager config = new ConfigManager();

        public static string SettingsFolderPath { get; } = config.GetAppSetting("SettingsFolderPath");
        public static string SettingsFile { get; } = config.GetAppSetting("SettingsNationalityFile");

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
                    Actors.Nationality? retValue = converter.Convert(item.Name);

                    Assert.IsTrue(retValue.HasValue, $"The Nationality string \"{item.Name}\" didn't translate into a Nationality type domain object.");
                }
            }
        }

        [TestMethod, TestCategory("Settings"), TestCategory("Converter"), TestCategory("Nationality Converter")]
        public void IncorrectNationalityStringIsNotTransformedIntoDomainObjects()
        {
            INationalityConverter converter = new NationalityConverter();
            Actors.Nationality? retValue = converter.Convert("Stupid name that doesn't exist in the file");
            Assert.IsFalse(retValue.HasValue, $"Value should have been false because the supplied name doesn't exist.");

            retValue = converter.Convert(null);
            Assert.IsFalse(retValue.HasValue, $"Value should have been null because false was supplied as the value to translate.");
        }
    }
}
