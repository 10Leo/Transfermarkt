using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts.Converter;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    public class FootConverter : IFootConverter
    {
        private static IConfigurationManager config = new ConfigManager();
        private ILogger logger;

        public static string Language { get; } = config.GetAppSetting("Language");
        public static string SettingsFolderPath { get; } = config.GetAppSetting("SettingsFolderPath");
        public static string SettingsFile { get; } = config.GetAppSetting("SettingsFootFile");

        private readonly IDictionary<string, Foot> map = new Dictionary<string, Foot>();

        public FootConverter() : this(null) { }

        public FootConverter(ILogger logger)
        {
            this.logger = logger;
            Load();
        }

        public FootValue Convert(string stringToConvert)
        {
            Foot? p = null;
            try
            {
                p = map[stringToConvert];
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogException(LogLevel.Error, $"The string {stringToConvert} wasn't found on the config file.", ex);
            }
            catch (ArgumentNullException ex)
            {
                logger.LogException(LogLevel.Error, $"Null argument string {stringToConvert} passed.", ex);
            }
            return new FootValue { Value = p };
        }

        private void Load()
        {
            string language = Config.GetLanguageFolder(Language);
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{SettingsFile}");

            var definition = new { Language = default(string), Set = new[] { new { ID = default(int), Name = default(string), DO = default(string) } } };
            var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            deserializedJSON.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Foot toDomainObject);
                map.Add(p.Name, toDomainObject);
            });
        }
    }
}
