using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core.Exporter
{
    public class JsonExporter
    {
        private static readonly string format = ".json";
        private static readonly string dateFormat = "yyyy-MM-dd";
        private static readonly string baseFolderPath = @"c:\TM";
        private static readonly string baseFolderName = "TM";

        private static readonly string competitionFileFormat = "{COUNTRY}-{COMPETITION_NAME}" + format;
        private static readonly string clubFileFormat = "{COUNTRY}-{CLUB_NAME}" + format;

        private static JsonSerializerSettings settings;

        static JsonExporter() {
            settings = new JsonSerializerSettings
            {
                DateFormatString = dateFormat,
            };
            settings.Formatting = Formatting.Indented;
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        }

        public static void ExtractCompetition(Competition competition)
        {
            string pathString = CreateBaseDir();

            string fileName = clubFileFormat;
            fileName = clubFileFormat.Replace("{COUNTRY}", competition.Country?.ToString());
            fileName = fileName.Replace("{COMPETITION_NAME}", competition.Name);

            pathString = System.IO.Path.Combine(pathString, fileName);

            string output = JsonConvert.SerializeObject(competition, settings);

            WriteToFile(pathString, output);
        }

        public static void ExtractClub(Club club)
        {
            string pathString = CreateBaseDir();

            pathString = System.IO.Path.Combine(pathString, string.Format("{0}", club.Country));
            System.IO.Directory.CreateDirectory(pathString);

            string fileName = clubFileFormat;
            fileName = clubFileFormat.Replace("{COUNTRY}", club.Country?.ToString());
            fileName = fileName.Replace("{CLUB_NAME}", club.Name);

            pathString = System.IO.Path.Combine(pathString, fileName);

            string output = JsonConvert.SerializeObject(club, settings);

            WriteToFile(pathString, output);
        }

        private static string CreateBaseDir()
        {
            string pathString = System.IO.Path.Combine(baseFolderPath, string.Format("{0}{1}", baseFolderName, DateTime.Today.ToString("yyyyMMdd")));
            System.IO.Directory.CreateDirectory(pathString);

            return pathString;
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
