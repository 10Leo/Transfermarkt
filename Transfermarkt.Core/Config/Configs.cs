using System.Configuration;

namespace Transfermarkt.Core
{
    public static class Keys
    {
        public struct Config
        {
            public const string Language = "Language";
            public const string BaseURL = "BaseURL";
            public const string BaseFolderPath = "BaseFolderPath";
            public const string OutputFolderPath = "OutputFolderPath";

            public const string SettingsFolderPath = "SettingsFolderPath";
            public const string SettingsPositionFile = "SettingsPositionFile";
            public const string SettingsNationalityFile = "SettingsNationalityFile";
            public const string SettingsFootFile = "SettingsFootFile";
            public const string SettingsContinentFile = "SettingsContinentFile";

            public const string Level1FolderFormat = "Level1FolderFormat";

            public const string ContinentFileNameFormat = "ContinentFileNameFormat";
            public const string CompetitionFileNameFormat = "CompetitionFileNameFormat";
            public const string ClubFileNameFormat = "ClubFileNameFormat";

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

    public enum ConfigType
    {
        PLAYER,
        CLUB,
        COMPETITION,
        CONTINENT
    }
}
