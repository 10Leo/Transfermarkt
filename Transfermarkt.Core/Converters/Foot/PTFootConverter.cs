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
    public class PTFootConverter : IFootConverter
    {
        public Foot? Convert(string stringToConvert)
        {
            return Converter(stringToConvert);
        }

        private Foot? Converter(string str)
        {
            switch (str)
            {
                case "direito": return Foot.R;
                case "esquerdo": return Foot.L;
                case "ambos": return Foot.A;
                default: return null;
            }
        }
    }
}
