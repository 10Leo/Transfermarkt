using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
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
        public static string ContinentFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ContinentFileNameFormat);
        public static string CompetitionFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.CompetitionFileNameFormat);
        public static string ClubFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ClubFileNameFormat);

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
            string output = o?.ToString();

            if (string.IsNullOrEmpty(output))
            {
                return;
            }

            string pathString = CreateBaseDir();

            
            string fileName = string.Empty;
            if (domain is Continent)
            {
                fileName = GenerateFileName(ContinentFileNameFormat, domain);
            }
            else if(domain is Competition)
            {
                fileName = GenerateFileName(CompetitionFileNameFormat, domain);
            }
            else if (domain is Club)
            {
                fileName = GenerateFileName(ClubFileNameFormat, domain);

                var country = (NationalityValue)domain.Elements.FirstOrDefault(e => e.InternalName == "Country")?.Value;
                if (country != null && country.Value.HasValue)
                {
                    pathString = System.IO.Path.Combine(pathString, string.Format("{0}", country.Value.ToString()));
                    System.IO.Directory.CreateDirectory(pathString);
                }
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }
            pathString = System.IO.Path.Combine(pathString, fileName);

            WriteToFile(pathString, output);
        }

        public JObject Extract(JObject baseObj, IDomain<IValue> domain)
        {
            foreach (IElement<IValue> element in domain.Elements)
            {
                object value = GetValue(element);
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

            var setProp = new JProperty("Children", set);
            baseObj.Add(setProp);

            return baseObj;
        }

        private object GetValue(IElement<IValue> element)
        {
            object value = string.Empty;

            if (element == null)
            {
                return value;
            }

            if (element.Value.Type == typeof(int?))
            {
                value = ((IntValue)element.Value).Value;
            }
            else if (element.Value.Type == typeof(decimal?))
            {
                value = ((DecimalValue)element.Value).Value;
            }
            else if (element.Value.Type == typeof(DateTime?))
            {
                value = ((DatetimeValue)element.Value).Value?.ToString("dd.MM.yyyy");
            }
            else if (element.Value.Type == typeof(string))
            {
                value = ((StringValue)element.Value).Value;
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
            else if (element.Value.Type == typeof(ContinentCode?))
            {
                value = ((ContinentCodeValue)element.Value).Value?.ToString();
            }
            else
            {
                value = element.Value?.ToString();
            }

            return value;
        }

        private string GenerateFileName(string template, IDomain<IValue> domain)
        {
            string fileName = template;

            Regex r = new Regex(@"{(?<Key>[^\}]+)}");
            var keys = r.Matches(template);

            if (keys == null || keys.Count == 0)
            {
                return template;
            }

            foreach (Match key in keys)
            {
                var k = key.Groups["Key"];
                var v = k.Value;

                IElement<IValue> element = domain.Elements.FirstOrDefault(e => e.InternalName.ToUpperInvariant() == v.ToUpperInvariant());
                string value = GetValue(element)?.ToString();

                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    value = "-";
                }

                fileName = fileName.Replace($"{{{k}}}", value);
            }

            fileName = MakeValidFileName(fileName) + format;

            return fileName;
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
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
