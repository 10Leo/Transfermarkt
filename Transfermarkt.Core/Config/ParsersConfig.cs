using LJMB.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Transfermarkt.Core
{
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
            string language = Config.GetLanguageFolder(Language);

            PlayerMap(language);
            ClubMap(language);
            CompetitionMap(language);
            ContinentMap(language);
        }

        #region Public

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

        #endregion Public

        #region Private

        private static void PlayerMap(string language)
        {
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\Parsers\{PlayerSettingsFile}");

            var definition = new { Language = default(string), Set = new[] { new { ID = default(int), Name = default(string), DO = default(string) } } };

            var playerDeserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            playerDeserializedJSON.Set.ToList().ForEach(p =>
            {
                // TODO: consider changing this to remove the existing dependency of the Parser's class name, that must end with Parser.
                playerMap.Add(p.DO + "Parser", p.Name);
            });
        }

        private static void ClubMap(string language)
        {
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\Parsers\{ClubSettingsFile}");
            var definition = new { Language = default(string), Set = new[] { new { ID = default(int), Name = default(string), DO = default(string) } } };

            var clubDeserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            clubDeserializedJSON.Set.ToList().ForEach(p =>
            {
                clubMap.Add(p.DO + "Parser", p.Name);
            });

        }

        private static void CompetitionMap(string language)
        {
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\Parsers\{CompetitionSettingsFile}");
            var definition = new { Language = default(string), Set = new[] { new { ID = default(int), Name = default(string), DO = default(string) } } };

            var competitionDeserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            competitionDeserializedJSON.Set.ToList().ForEach(p =>
            {
                competitionMap.Add(p.DO + "Parser", p.Name);
            });
        }

        private static void ContinentMap(string language)
        {
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\Parsers\{ContinentSettingsFile}");
            var definition = new { Language = default(string), Set = new[] { new { ID = default(int), Name = default(string), DO = default(string) } } };

            var continentDeserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            continentDeserializedJSON.Set.ToList().ForEach(p =>
            {
                continentMap.Add(p.DO + "Parser", p.Name);
            });
        }

        #endregion Private
    }
}
