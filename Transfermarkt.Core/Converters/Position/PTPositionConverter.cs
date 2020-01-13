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
    public class PTPositionConverter : Position.PositionConverter, IPositionConverter
    {
        public PTPositionConverter()
        {
            positions = JsonConvert.DeserializeObject<PositionsJSON>(File.ReadAllText($@"{SettingsFolderPath}\PT\{SettingsPositionsFile}"));
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
