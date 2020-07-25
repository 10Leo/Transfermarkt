using LJMB.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core
{
    public static class ConvertersConfig
    {
        public static string Language { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.Language);
        public static string SettingsFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SettingsFolderPath);
        public static string NationalitySettingsFile { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SettingsNationalityFile);
        public static string FootSettingsFile { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SettingsFootFile);
        public static string PositionSettingsFile { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SettingsPositionFile);
        public static string ContinentSettingsFile { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SettingsContinentFile);

        private static readonly IDictionary<string, Nationality> nationalityMap = new Dictionary<string, Nationality>();
        private static readonly IDictionary<string, Position> positionMap = new Dictionary<string, Position>();
        private static readonly IDictionary<string, Foot> footMap = new Dictionary<string, Foot>();
        private static readonly IDictionary<string, ContinentCode[]> continentMap = new Dictionary<string, ContinentCode[]>();

        static ConvertersConfig()
        {
            string language = Config.GetLanguageFolder(Language);

            NationalityConfigure(language);
            PositionConfigure(language);
            FootConfigure(language);
            ContinentConfigure(language);
        }

        #region Public

        public static Nationality? GetNationality(string key)
        {
            if (key == null || !nationalityMap.ContainsKey(key))
            {
                return null;
            }
            return nationalityMap[key];
        }

        public static Position? GetPosition(string key)
        {
            if (key == null || !positionMap.ContainsKey(key))
            {
                return null;
            }
            return positionMap[key];
        }

        public static Foot? GetFoot(string key)
        {
            if (key == null || !footMap.ContainsKey(key))
            {
                return null;
            }
            return footMap[key];
        }

        public static ContinentCode? GetContinent(string key)
        {
            if (key == null || !continentMap.ContainsKey(key))
            {
                return null;
            }
            return continentMap[key].FirstOrDefault();
        }

        #endregion Public

        #region Private

        private static void NationalityConfigure(string language)
        {
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{NationalitySettingsFile}");
            var definition = new { Language = default(string), Set = new[] { new { ID = default(string), Name = default(string), DO = default(string) } } };
            var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            deserializedJSON.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Nationality toDomainObject);
                nationalityMap.Add(p.Name, toDomainObject);
            });
        }

        private static void PositionConfigure(string language)
        {
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{PositionSettingsFile}");
            var definition = new { Language = default(string), Set = new[] { new { ID = default(string), Name = default(string), DO = default(string) } } };
            var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            deserializedJSON.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Position toDomainObject);
                positionMap.Add(p.Name, toDomainObject);
            });
        }

        private static void FootConfigure(string language)
        {
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{FootSettingsFile}");
            var definition = new { Language = default(string), Set = new[] { new { ID = default(string), Name = default(string), DO = default(string) } } };
            var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            deserializedJSON.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Foot toDomainObject);
                footMap.Add(p.Name, toDomainObject);
            });
        }

        private static void ContinentConfigure(string language)
        {
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{ContinentSettingsFile}");
            var definitionContinent = new
            {
                Language = default(string),
                Set = new[] {
                    new {
                        ID = default(string),
                        Name = default(string),
                        DO = new[] {
                            default(string)
                        }
                    }
                }
            };
            var deserializedJSONContinent = JsonConvert.DeserializeAnonymousType(json, definitionContinent);
            deserializedJSONContinent.Set.ToList().ForEach(p =>
            {
                var ccs = new List<ContinentCode>();
                p.DO.ToList().ForEach(d =>
                {
                    Enum.TryParse(d, out ContinentCode toDomainObject);
                    ccs.Add(toDomainObject);
                });
                continentMap.Add(p.Name, ccs.ToArray());
            });
        }

        #endregion Private
    }
}
