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
    public class PTPositionConverter : IPositionConverter
    {
        [JsonObject(Title = "Positions")]
        private class PositionsJSON
        {
            public string Language { get; set; }
            public IList<PositionJSON> Positions { get; set; }
        }

        [JsonObject(Title = "Position")]
        private class PositionJSON
        {
            public string Name { get; set; }
            public string DO { get; set; }
        }

        private static readonly string dateFormat = "yyyy-MM-dd";

        public static string SettingsPTFolderPath { get; } = ConfigurationManager.AppSettings["SettingsPTFolderPath"].ToString();
        public static string SettingsPTPositionsFile { get; } = ConfigurationManager.AppSettings["SettingsPTPositionsFile"].ToString();

        private static JsonSerializerSettings settings;
        private readonly PositionsJSON positions;

        private readonly IDictionary<string, Position> positionMapper = new Dictionary<string, Position>();

        public PTPositionConverter()
        {
            settings = new JsonSerializerSettings
            {
                DateFormatString = dateFormat,
            };
            settings.Formatting = Formatting.Indented;
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            positions = JsonConvert.DeserializeObject<PositionsJSON>(File.ReadAllText($@"{SettingsPTFolderPath}\{SettingsPTPositionsFile}"));
            positions.Positions.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Position toDomainObject);
                positionMapper.Add(p.Name, toDomainObject);
            });
        }

        public Position? Convert(string stringToConvert)
        {
            Position? p = null;
            try
            {
                p = positionMapper[stringToConvert];
            }
            catch (KeyNotFoundException ex)
            {
                //log
            }
            catch(ArgumentNullException ex) { }
            return p;
        }
    }
}
