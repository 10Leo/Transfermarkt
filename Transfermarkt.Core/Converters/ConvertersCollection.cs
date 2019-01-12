using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts.Converters;

namespace Transfermarkt.Core.Converters
{
    public class ConvertersCollection
    {
        public ConvertersCollection(INationalityConverter nationalityConverter, IPositionConverter positionConverter, IFootConverter footConverter)
        {
            NationalityConverter = nationalityConverter;
            PositionConverter = positionConverter;
            FootConverter = footConverter;
        }

        public INationalityConverter NationalityConverter { get; set; }
        public IPositionConverter PositionConverter { get; set; }
        public IFootConverter FootConverter { get; set; }
    }
}
