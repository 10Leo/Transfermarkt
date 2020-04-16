using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Transfermarkt.Core;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Exporter;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Exporter.JSONExporter
{
    public class JsonExporter : IExporter
    {
        private static readonly string dateFormat = "yyyy-MM-dd";
        private static readonly string format = ".json";
        private static readonly string competitionFileFormat = "{COUNTRY}-{COMPETITION_NAME}_{SEASON}" + format;
        private static readonly string clubFileFormat = "{COUNTRY}-{CLUB_NAME}_{SEASON}" + format;

        public static string BaseFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseFolderPath);
        public static string Level1FolderFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.Level1FolderFormat);


        private static JsonSerializerSettings settings;

        public JsonExporter()
        {
            settings = new JsonSerializerSettings
            {
                DateFormatString = dateFormat,
            };
            settings.Formatting = Formatting.Indented;
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        }

        public void Extract(IDomain<IValue> domain)
        {
            var o = Extract(new JObject(), domain);

            string output = o.ToString();
        }

        public JObject Extract(JObject baseObj, IDomain<IValue> domain)
        {
            //JObject jObject = new JObject();

            foreach (IElement<IValue> element in domain.Elements)
            {
                string value = string.Empty;
                if (element.Value.Type == typeof(string))
                {
                    value = ((StringValue)element.Value).Value;
                }
                else if (element.Value.Type == typeof(int?))
                {
                    value = ((IntValue)element.Value).Value?.ToString();
                }
                else if (element.Value.Type == typeof(decimal?))
                {
                    value = ((DecimalValue)element.Value).Value?.ToString();
                }
                else if (element.Value.Type == typeof(DateTime?))
                {
                    value = ((DatetimeValue)element.Value).Value?.ToString();
                }
                else if (element.Value.Type == typeof(Nationality?))
                {
                    value = ((NationalityValue)element.Value).Value?.ToString();
                }
                else if (element.Value.Type == typeof(Position?))
                {
                    value = ((PositionValue)element.Value).Value?.ToString();
                }
                else if (element.Value.Type == typeof(Foot?))
                {
                    value = ((FootValue)element.Value).Value?.ToString();
                }

                var prop = new JProperty(element.InternalName, value);
                baseObj.Add(prop);
            }

            if (domain.Children == null || domain.Children.Count == 0)
            {
                return baseObj;
            }

            var set = new JArray();

            foreach (var child in domain.Children)
            {
                var o = Extract(new JObject(), child);
                set.Add(o);
            }

            var setProp = new JProperty("Set", set);
            baseObj.Add(setProp);

            return baseObj;
        }

        public void ExtractCompetition(Competition competition)
        {
            string pathString = CreateBaseDir();

            string fileName = competitionFileFormat;
            //fileName = fileName.Replace("{COUNTRY}", competition.Country?.ToString());
            //fileName = fileName.Replace("{COMPETITION_NAME}", competition.Name);
            //fileName = fileName.Replace("{SEASON}", competition.Season.ToString());

            pathString = System.IO.Path.Combine(pathString, fileName);

            string output = JsonConvert.SerializeObject(competition, settings);

            WriteToFile(pathString, output);
        }

        public void ExtractClub(Club club)
        {
            //string pathString = CreateBaseDir();

            //pathString = System.IO.Path.Combine(pathString, string.Format("{0}", club.Country));
            //System.IO.Directory.CreateDirectory(pathString);

            //string fileName = clubFileFormat;
            //fileName = fileName.Replace("{COUNTRY}", club.Country?.ToString());
            //fileName = fileName.Replace("{CLUB_NAME}", club.Name);
            //fileName = fileName.Replace("{SEASON}", club.Season.ToString());

            //pathString = System.IO.Path.Combine(pathString, fileName);

            //string output = JsonConvert.SerializeObject(club, settings);

            //WriteToFile(pathString, output);
        }

        private static string CreateBaseDir()
        {
            string level1FolderName = Level1FolderFormat.Replace("{0}", DateTime.Today.ToString("yyyyMMdd"));
            string level1PathString = System.IO.Path.Combine(BaseFolderPath, level1FolderName);

            System.IO.Directory.CreateDirectory(level1PathString);

            return level1PathString;
        }

        private static void WriteToFile(string filePath, string output)
        {
            //byte[] bytes = Encoding.ASCII.GetBytes(output);

            if (!System.IO.File.Exists(filePath))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(filePath))
                {
                    //foreach (byte b in bytes)
                    //{
                    //    fs.WriteByte(b);
                    //}
                }
            }

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath))
            {
                sw.Write(output);
            }
        }
    }
}
