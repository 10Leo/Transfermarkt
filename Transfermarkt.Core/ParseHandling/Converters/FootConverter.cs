using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts.Converter;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    public class FootConverter : IFootConverter
    {
        private static IConfigurationManager config = new ConfigManager();

        public static string Language { get; } = config.GetAppSetting("Language");
        public static string SettingsFolderPath { get; } = config.GetAppSetting("SettingsFolderPath");
        public static string SettingsFile { get; } = config.GetAppSetting("SettingsFootFile");

        private readonly IDictionary<string, Foot> map = new Dictionary<string, Foot>();

        public FootConverter()
        {
            string language = GetLanguageFolder(Language);
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{SettingsFile}");

            var definition = new { Language = default(string), Set = new[] { new { ID = default(int), Name = default(string), DO = default(string) } } };
            var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            deserializedJSON.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Foot toDomainObject);
                map.Add(p.Name, toDomainObject);
            });
        }

        public FootValue Convert(string stringToConvert)
        {
            Foot? p = null;
            try
            {
                p = map[stringToConvert];
            }
            catch (KeyNotFoundException)
            {
                //TODO: log
            }
            catch (ArgumentNullException)
            {
                //TODO: log
            }
            return new FootValue { Value = p };
        }

        private string GetLanguageFolder(string language)
        {
            switch (language.ToLowerInvariant())
            {
                case "pt":
                    return "PT";
                case "en":
                    return "EN";
                default:
                    return null;
            }
        }
    }
}
