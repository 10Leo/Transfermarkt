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
            positions = JsonConvert.DeserializeObject<PositionsJSON>(File.ReadAllText($@"{SettingsFolderPath}\EN\{SettingsPositionsFile}"));
            positions.Positions.ToList().ForEach(p =>
            {
                Enum.TryParse(p.DO, out Actors.Position toDomainObject);
                positionMapper.Add(p.Name, toDomainObject);
            });
        }

        public override Actors.Position? Convert(string stringToConvert)
        {
            return base.Convert(stringToConvert);
        }
    }
}
