using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts.Converters;

namespace Transfermarkt.Core.Converters.Position
{
    public abstract class PositionConverter : IPositionConverter
    {
        [JsonObject(Title = "Positions")]
        protected class PositionsJSON
        {
            public string Language { get; set; }
            public IList<PositionJSON> Positions { get; set; }
        }

        [JsonObject(Title = "Position")]
        protected class PositionJSON
        {
            public string Name { get; set; }
            public string DO { get; set; }
        }

        protected static readonly string dateFormat = "yyyy-MM-dd";

        public static string SettingsFolderPath { get; } = ConfigurationManager.AppSettings["SettingsFolderPath"].ToString();
        public static string SettingsPositionsFile { get; } = ConfigurationManager.AppSettings["SettingsPositionsFile"].ToString();

        protected static JsonSerializerSettings settings;
        protected PositionsJSON positions;

        protected readonly IDictionary<string, Actors.Position> positionMapper = new Dictionary<string, Actors.Position>();

        public PositionConverter()
        {
            settings = new JsonSerializerSettings
            {
                DateFormatString = dateFormat,
            };
            settings.Formatting = Formatting.Indented;
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        }

        public virtual Actors.Position? Convert(string stringToConvert)
        {
            Actors.Position? p = null;
            try
            {
                p = positionMapper[stringToConvert];
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
