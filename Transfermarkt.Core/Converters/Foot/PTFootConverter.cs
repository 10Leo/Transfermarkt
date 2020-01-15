using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static readonly string language = "PT";
        private static readonly string dateFormat = "yyyy-MM-dd";

        public static string SettingsFolderPath { get; } = ConfigurationManager.AppSettings["SettingsFolderPath"].ToString();
        public static string SettingsFootFile { get; } = ConfigurationManager.AppSettings["SettingsFootFile"].ToString();

        private readonly IDictionary<string, Actors.Foot> map = new Dictionary<string, Actors.Foot>();

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
