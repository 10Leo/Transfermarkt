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
    public class PTNationalityConverter : INationalityConverter
    {
        public Nationality? Convert(string stringToConvert)
        {
            return Converter(stringToConvert);
        }

        private Nationality? Converter(string str)
        {
            switch (str)
            {
                case "Brasil": return Nationality.BRA;
                case "Inglaterra": return Nationality.GBR;
                case "Itália": return Nationality.ITA;
                case "Espanha": return Nationality.SPA;
                case "Portugal": return Nationality.PRT;
                default: return null;
            }
        }
    }
}
