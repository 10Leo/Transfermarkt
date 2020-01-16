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
    public class PTFootConverter : IFootConverter
    {
        private static readonly string language = "PT";
        private static readonly string dateFormat = "yyyy-MM-dd";

        public static string SettingsFolderPath { get; } = ConfigurationManager.AppSettings["SettingsFolderPath"].ToString();
        public static string SettingsFootFile { get; } = ConfigurationManager.AppSettings["SettingsFootFile"].ToString();

        private readonly IDictionary<string, Foot> map = new Dictionary<string, Foot>();

        public PTFootConverter()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DateFormatString = dateFormat,
            };
            settings.Formatting = Formatting.Indented;
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{SettingsFootFile}");

            var definition = new { Language = default(string), Set = new[] { new { ID = default(int), Name = default(string), DO = default(string) } } };
            var deserializedJSON = JsonConvert.DeserializeAnonymousType(json, definition);
            deserializedJSON.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Foot toDomainObject);
                map.Add(p.Name, toDomainObject);
            });
        }

        public Foot? Convert(string stringToConvert)
        {
            Foot? p = null;
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
