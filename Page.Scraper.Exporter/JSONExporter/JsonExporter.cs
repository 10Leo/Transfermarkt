using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Page.Scraper.Contracts;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Page.Scraper.Exporter.JSONExporter
{
    public class JsonExporter : IExporter
    {
        private static readonly string dateFormat = "yyyy-MM-dd";
        private static readonly string extension = ".json";
        private static JsonSerializerSettings settings;

        private string filename = string.Empty;
        private readonly string baseFolderPath = string.Empty;
        private readonly string level1FolderFormat = string.Empty;

        public JsonExporter(string baseFolderPath, string level1FolderFormat)
        {
            this.baseFolderPath = baseFolderPath;
            this.level1FolderFormat = level1FolderFormat;
            settings = new JsonSerializerSettings
            {
                DateFormatString = dateFormat,
            };
            settings.Formatting = Formatting.Indented;
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        }

        public void Extract(IDomain domain, string template)
        {
            this.filename = GenerateFileName(template, domain);

            var o = Extract(new JObject(), domain);
            string output = o?.ToString();

            if (string.IsNullOrEmpty(output))
            {
                return;
            }

            string pathString = CreateBaseDir(this.baseFolderPath, this.level1FolderFormat);

            if (string.IsNullOrWhiteSpace(this.filename))
            {
                return;
            }
            pathString = System.IO.Path.Combine(pathString, this.filename);

            WriteToFile(pathString, output);
        }

        private JObject Extract(JObject baseObj, IDomain domain)
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
            else
            {
                value = element.Value?.ToString();
            }

            return value;
        }

        private string GenerateFileName(string template, IDomain domain)
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

            fileName = MakeValidFileName(fileName) + extension;

            return fileName;
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }

        private static string CreateBaseDir(string baseFolderPath, string level1FolderFormat)
        {
            string level1FolderName = level1FolderFormat.Replace("{0}", DateTime.Today.ToString("yyyyMMdd"));
            string level1PathString = System.IO.Path.Combine(baseFolderPath, level1FolderName);

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
