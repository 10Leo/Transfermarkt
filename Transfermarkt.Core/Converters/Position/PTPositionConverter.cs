using System;
using System.Collections.Generic;
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
        public Position? Convert(string stringToConvert)
        {
            return Converter(stringToConvert);
        }

        private Position? Converter(string str)
        {
            switch (str)
            {
                case "Guarda-Redes": return Position.GR;
                default: return null;
            }
        }
    }
}
