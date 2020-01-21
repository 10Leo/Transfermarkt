using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts.Converters;

namespace Transfermarkt.Core.Converters
{
    public class NationalityConverter : INationalityConverter
    {
        public static string Language { get; } = ConfigurationManager.AppSettings["Language"].ToString();
        public static string SettingsFolderPath { get; } = ConfigurationManager.AppSettings["SettingsFolderPath"].ToString();
        public static string SettingsFile { get; } = ConfigurationManager.AppSettings["SettingsNationalityFile"].ToString();

        private readonly IDictionary<string, Nationality> map = new Dictionary<string, Nationality>();

        public NationalityConverter()
        {
            string language = GetLanguageFolder(Language);
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{SettingsFile}");

            var definition = new { Language = default(string), Set = new[] { new { ID = default(string), Name = default(string), DO = default(string) } } };
            var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            deserializedJSON.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Nationality toDomainObject);
                map.Add(p.Name, toDomainObject);
            });
        }

        public Nationality? Convert(string stringToConvert)
        {
            Nationality? p = null;
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
            return p;
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
