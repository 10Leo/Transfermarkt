using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core
{
    public class ConfigManager : IConfigurationManager
    {
        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key]?.ToString();
        }
    }

    public static class Config
    {
        public static string GetLanguageFolder(string language)
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

    public static class ConvertersConfig
    {
        private static IConfigurationManager config = new ConfigManager();

        public static string Language { get; } = config.GetAppSetting("Language");
        public static string SettingsFolderPath { get; } = config.GetAppSetting("SettingsFolderPath");
        public static string NationalitySettingsFile { get; } = config.GetAppSetting("SettingsNationalityFile");
        public static string FootSettingsFile { get; } = config.GetAppSetting("SettingsFootFile");
        public static string PositionSettingsFile { get; } = config.GetAppSetting("SettingsPositionFile");

        private static readonly IDictionary<string, Nationality> nationalityMap = new Dictionary<string, Nationality>();
        private static readonly IDictionary<string, Position> positionMap = new Dictionary<string, Position>();
        private static readonly IDictionary<string, Foot> footMap = new Dictionary<string, Foot>();

        static ConvertersConfig()
        {
            string language = Config.GetLanguageFolder(Language);
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{NationalitySettingsFile}");

            var definition = new { Language = default(string), Set = new[] { new { ID = default(string), Name = default(string), DO = default(string) } } };
            var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            deserializedJSON.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Nationality toDomainObject);
                nationalityMap.Add(p.Name, toDomainObject);
            });

            json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{PositionSettingsFile}");
            deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            deserializedJSON.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Position toDomainObject);
                positionMap.Add(p.Name, toDomainObject);
            });

            json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{FootSettingsFile}");
            deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            deserializedJSON.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Foot toDomainObject);
                footMap.Add(p.Name, toDomainObject);
            });
        }

        public static Nationality? GetNationality(string key)
        {
            return nationalityMap[key];
        }

        public static Position? GetPosition(string key)
        {
            return positionMap[key];
        }

        public static Foot? GetFoot(string key)
        {
            return footMap[key];
        }
    }

    public static class ParsersConfig
    {
        private static readonly IConfigurationManager config = new ConfigManager();

        public static string Language { get; } = config.GetAppSetting("Language");
        public static string SettingsFolderPath { get; } = config.GetAppSetting("SettingsFolderPath");
        public static string PlayerSettingsFile { get; } = "player.json";
        public static string ClubSettingsFile { get; } = "club.json";
        public static string CompetitionSettingsFile { get; } = "competition.json";
        public static string ContinentSettingsFile { get; } = "continent.json";

        private static readonly IDictionary<string, string> playerMap = new Dictionary<string, string>();
        private static readonly IDictionary<string, string> clubMap = new Dictionary<string, string>();
        private static readonly IDictionary<string, string> competitionMap = new Dictionary<string, string>();
        private static readonly IDictionary<string, string> continentMap = new Dictionary<string, string>();

        static ParsersConfig()
        {
            string name = string.Empty;
            string language = Config.GetLanguageFolder(Language);
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\Parsers\{PlayerSettingsFile}");

            var definition = new { Language = default(string), Set = new[] { new { ID = default(int), Name = default(string), DO = default(string) } } };

            var playerDeserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            playerDeserializedJSON.Set.ToList().ForEach(p =>
            {
                playerMap.Add(p.DO + "Parser", p.Name);
            });

            json = File.ReadAllText($@"{SettingsFolderPath}\{language}\Parsers\{ClubSettingsFile}");
            var clubDeserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            clubDeserializedJSON.Set.ToList().ForEach(p =>
            {
                clubMap.Add(p.DO + "Parser", p.Name);
            });

            json = File.ReadAllText($@"{SettingsFolderPath}\{language}\Parsers\{CompetitionSettingsFile}");
            var competitionDeserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            competitionDeserializedJSON.Set.ToList().ForEach(p =>
            {
                competitionMap.Add(p.DO + "Parser", p.Name);
            });

            json = File.ReadAllText($@"{SettingsFolderPath}\{language}\Parsers\{ContinentSettingsFile}");
            var continentDeserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            continentDeserializedJSON.Set.ToList().ForEach(p =>
            {
                continentMap.Add(p.DO + "Parser", p.Name);
            });
        }

        public static string Get(Type type, ConfigType level)
        {
            string p = string.Empty;

            switch (level)
            {
                case ConfigType.PLAYER:
                    p = playerMap[type.Name];
                    break;
                case ConfigType.CLUB:
                    p = clubMap[type.Name];
                    break;
                case ConfigType.COMPETITION:
                    p = competitionMap[type.Name];
                    break;
                case ConfigType.CONTINENT:
                    p = continentMap[type.Name];
                    break;
                default:
                    break;
            }
            return p;
        }
    }

    public enum ConfigType
    {
        PLAYER,
        CLUB,
        COMPETITION,
        CONTINENT
    }
}
