using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.Contracts.Converters;

namespace Transfermarkt.Core.Converters
{
    public class PTNationalityConverter : INationalityConverter
    {
        private static readonly string language = "PT";
        private static readonly string dateFormat = "yyyy-MM-dd";

        public static string SettingsFolderPath { get; } = ConfigurationManager.AppSettings["SettingsFolderPath"].ToString();
        public static string SettingsNationalitiesFile { get; } = ConfigurationManager.AppSettings["SettingsNationalitiesFile"].ToString();

        private readonly IDictionary<string, Actors.Nationality> map = new Dictionary<string, Actors.Nationality>();

        public PTNationalityConverter()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DateFormatString = dateFormat,
            };
            settings.Formatting = Formatting.Indented;
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{SettingsNationalitiesFile}");

            var definition = new { Language = default(string), Set = new[] { new { Name = default(string), DO = default(string) } } };
            var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            deserializedJSON.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Actors.Nationality toDomainObject);
                map.Add(p.Name, toDomainObject);
            });
        }

        public Nationality? Convert(string stringToConvert)
        {
            Actors.Nationality? p = null;
            try
            {
                p = map[stringToConvert];
            }
            catch (KeyNotFoundException)
            {
                //log
            }
            catch (ArgumentNullException)
            {
                //log
            }
            return p;
        }
    }
}
