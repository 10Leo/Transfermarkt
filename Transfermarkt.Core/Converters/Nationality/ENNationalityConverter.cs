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
    public class ENNationalityConverter : INationalityConverter
    {
        private static readonly string language = "EN";

        public static string SettingsFolderPath { get; } = ConfigurationManager.AppSettings["SettingsFolderPath"].ToString();
        public static string SettingsNationalityFile { get; } = ConfigurationManager.AppSettings["SettingsNationalityFile"].ToString();

        private readonly IDictionary<string, Nationality> map = new Dictionary<string, Nationality>();

        public ENNationalityConverter()
        {
            string json = File.ReadAllText($@"{SettingsFolderPath}\{language}\{SettingsNationalityFile}");

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
