using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core
{
    public static class ConfigManager
    {
        public static T GetAppSetting<T>(string key) where T : IConvertible
        {
            T result = default(T);

            if (ConfigurationManager.AppSettings[key] == null)// || String.IsNullOrEmpty(td.Attributes[key].Value) == false)
            {
                return result;
            }

            string value = ConfigurationManager.AppSettings[key];

            try
            {
                result = (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                result = default(T);
            }

            return result;
        }
    }

    public static class Keys
    {
        public struct Config
        {
            public const string Language = "Language";
            public const string BaseURL = "BaseURL";
            public const string BaseFolderPath = "BaseFolderPath";
            public const string SettingsFolderPath = "SettingsFolderPath";
            public const string SettingsPositionFile = "SettingsPositionFile";
            public const string SettingsNationalityFile = "SettingsNationalityFile";
            public const string SettingsFootFile = "SettingsFootFile";
            public const string Level1FolderFormat = "Level1FolderFormat";
            public const string SimpleClubUrlFormat = "SimpleClubUrlFormat";
            public const string PlusClubUrlFormat = "PlusClubUrlFormat";
            public const string PlusClubUrlFormatV2 = "PlusClubUrlFormatV2";
            public const string CompetitionUrlFormat = "CompetitionUrlFormat";
            public const string IdentifiersGetterPattern = "IdentifiersGetterPattern";
            public const string IdentifiersSetterPattern = "IdentifiersSetterPattern";
            public const string LogPath = "LogPath";
            public const string MinimumLoggingLevel = "MinimumLoggingLevel";
        }
    }

    public struct Common
    {
        public const string date = @"yyyy.MM.dd";
        public static readonly char[] trimChars = new char[] { ' ', '\t', '\n', '\r' };

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
        public static string Language { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.Language);
        public static string SettingsFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SettingsFolderPath);
        public static string NationalitySettingsFile { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SettingsNationalityFile);
        public static string FootSettingsFile { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SettingsFootFile);
        public static string PositionSettingsFile { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SettingsPositionFile);

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
        public static string Language { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.Language);
        public static string SettingsFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SettingsFolderPath);
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
                // TODO: consider changing this to remove the existing dependency of the Parser's class name, that must end with Parser.
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

        public static string GetLabel(Type type, ConfigType level)
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
