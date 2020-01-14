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
    public class ENPositionConverter : Position.PositionConverter, IPositionConverter
    {
        public ENPositionConverter()
        {
            PositionsJSON positions = JsonConvert.DeserializeObject<PositionsJSON>(File.ReadAllText($@"{SettingsFolderPath}\EN\{SettingsPositionsFile}"));
            positions.Set.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Actors.Position toDomainObject);
                map.Add(p.Name, toDomainObject);
            });
        }

        public override Actors.Position? Convert(string stringToConvert)
        {
            return base.Convert(stringToConvert);
        }
    }
}
