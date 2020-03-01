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
    public class PositionConverter : IPositionConverter
    {
        private static IConfigurationManager config = new ConfigManager();
        private ILogger logger;

        public static string Language { get; } = config.GetAppSetting("Language");
        public static string SettingsFolderPath { get; } = config.GetAppSetting("SettingsFolderPath");
        public static string SettingsFile { get; } = config.GetAppSetting("SettingsPositionFile");

        private readonly IDictionary<string, Position> map = new Dictionary<string, Position>();

        public PositionConverter() : this(null) { }

        public PositionConverter(ILogger logger)
        {
            this.logger = logger;
            Load();
        }

        public PositionValue Convert(string stringToConvert)
        {
            Position? p = null;
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
            return new PositionValue { Value = p };
        }

        private void Load()
        {
            string language = Config.GetLanguageFolder(Language);
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{SettingsFile}");

            var definition = new { Language = default(string), Set = new[] { new { ID = default(string), Name = default(string), DO = default(string) } } };
            var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            deserializedJSON.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Position toDomainObject);
                map.Add(p.Name, toDomainObject);
            });
        }
    }
}
