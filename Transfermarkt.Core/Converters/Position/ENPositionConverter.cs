using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Converters
{
    public class ENPositionConverter : IPositionConverter
    {
        public Position? Convert(string stringToConvert)
        {
            throw new NotImplementedException();
        }

        private Position? Converter(string str)
        {
            switch (str)
            {
                default:
                    return null;
            }
        }
    }
}
