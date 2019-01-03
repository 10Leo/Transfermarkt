using HtmlAgilityPack;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Connectors
{
    public class HtmlAgilityPackConnector : IConnector
    {
        private HtmlDocument doc;

        public void ConnectToPage(string url)
        {
            try
            {
                string htmlCode = "";
                using (WebClient client = new WebClient())
                {
                    client.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
                    client.Headers.Add(HttpRequestHeader.UserAgent, "AvoidError");
                    htmlCode = client.DownloadString(url);
                }
                doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlCode);
            }
            catch (System.Net.WebException ex)
            {
                Debug.WriteLine(ex.StackTrace);
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        public DataTable GetCompetitionTable()
        {
            //Validate();

            string id = "items";

            HtmlNode table = doc.DocumentNode.SelectSingleNode("//div[@id='yw1']/table[@class='" + id + "']");

            if (table == null)
            {
                return null;
            }

            DataTable dataTable = new DataTable();
            
            foreach (string enumName in Enum.GetNames(typeof(CompetitionColumnsEnum)))
            {
                dataTable.Columns.Add(enumName);
            }

            var rows = table.SelectNodes(".//tbody/tr[td]");
            // each row is a player
            foreach (var row in rows)
            {
                //each column is an attribute
                HtmlNodeCollection cols = row.SelectNodes("td");

                try
                {
                    string clubUrl = GetClubUrl(cols[2]);

                    dataTable.Rows.Add(
                         clubUrl
                    );
                }
                catch (Exception ex)
                {
                }
            }

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (string enumName in Enum.GetNames(typeof(CompetitionColumnsEnum)))
                {
                    Console.Write(row[enumName].ToString());
                    Console.Write(", ");
                }
                Console.WriteLine("");
            }
            return dataTable;
        }

        public DataTable GetTableByID(string id)
        {
            Validate();

            HtmlNode table = doc.DocumentNode.SelectSingleNode("//table[@id='" + id + "']");
            return ParseTable(table);
        }

        public DataTable GetTableByClass(string className)
        {
            Validate();

            HtmlNode table = doc.DocumentNode.SelectSingleNode("//table[@class='" + className + "']");
            return ParseTable(table);
        }

        private DataTable ParseTable(HtmlNode table)
        {
            if (table == null)
            {
                return null;
            }

            DataTable dataTable = new DataTable();
            var headers = table.SelectNodes(".//thead/tr/th");
            var rows = table.SelectNodes(".//tbody/tr[td]");

            //foreach (HtmlNode header in h)
            //    dataTable.Columns.Add(header.InnerText);

            //foreach (var row in r)
            //    dataTable.Rows.Add(row.SelectNodes("td").Select(td => td.InnerText).ToArray());

            foreach (string enumName in Enum.GetNames(typeof(ClubColumnsEnum)))
            {
                dataTable.Columns.Add(enumName);
            }

            // each row is a player
            foreach (var row in rows)
            {
                //each column is an attribute
                HtmlNodeCollection cols = row.SelectNodes("td");

                try
                {
                    string profileUrl = GetProfileUrl(cols[1]);
                    string shirtNumber = GetShirtNumber(cols[0]);
                    string name = GetName(cols[1]);
                    string shortName = GetShortName(cols[1]);
                    string imgUrl = GetImgUrl(cols[1]);
                    string position = GetPosition(cols[1]);
                    string captain = GetCaptain(cols[1]);
                    string nationality = GetNationality(cols[3])?[0];
                    string birthDate = GetBirthDate(cols[2]);
                    string height = GetHeight(cols[4]);
                    string preferredFoot = GetPreferredFoot(cols[5]);
                    string clubArrivalDate = GetClubArrivalDate(cols[6]);
                    string contractExpirationDate = GetContractExpirationDate(cols[8]);
                    string marketValue = GetMarketValue(cols[9]);

                    dataTable.Rows.Add(
                          profileUrl
                        , shirtNumber
                        , name
                        , shortName
                        , imgUrl
                        , position
                        , captain
                        , nationality
                        , birthDate
                        , height
                        , preferredFoot
                        , clubArrivalDate
                        , contractExpirationDate
                        , marketValue
                    );
                }
                catch (Exception ex)
                {
                }
            }

            //foreach (DataRow row in dataTable.Rows)
            //{
            //    foreach (string enumName in Enum.GetNames(typeof(ClubColumnsEnum)))
            //    {
            //        Console.Write(row[enumName].ToString());
            //        Console.Write(", ");
            //    }
            //    Console.WriteLine("");
            //}
            return dataTable;
        }

        private string GetShirtNumber(HtmlNode node)
        {
            // node.Descendants("div")
            //    //.Select(d => d.Descendants()
            //    .Where(d => d.Attributes["class"].Value == "rn_nummer")
            //    .FirstOrDefault()
            //    .InnerText;
            return node
                .SelectNodes("div")
                .Where(n => n.Attributes["class"].Value == "rn_nummer")
                .FirstOrDefault()
                .InnerText;
        }
        private string GetProfileUrl(HtmlNode node)
        {
            return node
                .SelectNodes("table//td//a")
                .FirstOrDefault(n => n.Attributes["class"]?.Value == "spielprofil_tooltip")
                .Attributes["href"].Value;
        }
        private string GetName(HtmlNode node)
        {
            return node
                .SelectNodes("table//tr[1]/td[2]/div[1]")
                .FirstOrDefault()
                .InnerText;
        }
        private string GetShortName(HtmlNode node)
        {
            return node
                .SelectNodes("table//tr[1]/td[2]/div[2]")
                .FirstOrDefault()
                .InnerText;
        }
        private string GetImgUrl(HtmlNode node)
        {
            return "";
        }
        private string GetPosition(HtmlNode node)
        {
            return node
                .SelectNodes("table//tr[2]/td[1]")
                .FirstOrDefault()
                .InnerText;
        }
        private string GetCaptain(HtmlNode node)
        {
            var r = node
                .SelectNodes("table//tr[1]/td[2]/span");
            var cap = node
                .SelectNodes("table//tr[1]/td[2]/span")?
                .FirstOrDefault(n => (n.Attributes["class"]?.Value).Contains("kapitaenicon-table"));
            return (cap == null) ? "" : "c";
        }
        private string[] GetNationality(HtmlNode node)
        {
            return node
                .SelectNodes("img")
                .Where(n => n.Attributes["class"]?.Value == "flaggenrahmen")
                .Select(n => n.Attributes["title"].Value)?.ToArray();
        }
        private string GetBirthDate(HtmlNode node)
        {
            return node.InnerText?.Split(new[] { " (" }, StringSplitOptions.None)?[0];
        }
        private string GetHeight(HtmlNode node)
        {
            return Regex.Replace(node.InnerText, "([a-zA-Z,_ ]+|(?<=[a-zA-Z ])[/-])", "");
        }
        private string GetPreferredFoot(HtmlNode node)
        {
            return node
                .InnerText;
        }
        private string GetClubArrivalDate(HtmlNode node)
        {
            return node
                .InnerText;
        }
        private string GetContractExpirationDate(HtmlNode node)
        {
            return Regex.Replace(node.InnerText, @"\.", "/");
        }
        private string GetMarketValue(HtmlNode node)
        {
            var sp = node.InnerText?.Split(new[] { " " }, StringSplitOptions.None);

            if (sp == null || sp.Length < 2)
                return null;

            var spl = sp[0].Split(new[] { "," }, StringSplitOptions.None);
            int n = int.Parse(spl[0]);
            if (sp[1] == "M")
            {
                n = n * 1000000;
            } else if (sp[1] == "mil")
            {
                n = n * 1000;
            }
            return n.ToString();
        }

        private string GetClubUrl(HtmlNode node)
        {
            return node
                .SelectNodes("a")
                .FirstOrDefault(n => n.Attributes["class"]?.Value == "vereinprofil_tooltip")
                .Attributes["href"].Value;
        }

        private void Validate()
        {
            if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required
                throw new Exception("Erros no parse do doc");
            }

            if (doc.DocumentNode == null)
            {
                throw new Exception("Document Node null");
            }
        }
    }
}
