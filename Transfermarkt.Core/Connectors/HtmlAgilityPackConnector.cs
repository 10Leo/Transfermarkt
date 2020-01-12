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
    public class HtmlAgilityPackConnector : IPageConnector
    {
        private HtmlDocument doc;
        private string url;

        #region Contract

        public void ConnectToPage(string url)
        {
            this.url = url;
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

        public (string country, string countryImg, string Name, int Season, string ImgUrl) GetCompetitionData()
        {
            string name = doc.DocumentNode.SelectSingleNode("//div[@id='wettbewerb_head']//h1[@class='spielername-profil']")?.InnerText;
            string imgUrl = doc.DocumentNode.SelectSingleNode("//div[@id='wettbewerb_head']//div[@class='headerfoto']/img")?.GetAttributeValue<string>("src", null);

            int? season = doc.DocumentNode.SelectSingleNode("//select[@name='saison_id']//option")?.GetAttributeValue<int>("value", 0);
            if (!season.HasValue)
            {
                season = 0;
            }

            HtmlNode countryNode = doc.DocumentNode.SelectSingleNode("//div[@id='wettbewerb_head']//img[@class='flaggenrahmen']");
            string country = countryNode?.GetAttributeValue<string>("title", null);
            string countryUrl = countryNode?.GetAttributeValue<string>("src", null);

            return (country, countryUrl, name, (int)season, imgUrl);
        }

        public (string country, string countryImg, string Name, int Season, string ImgUrl) GetClubData()
        {
            string name = doc.DocumentNode.SelectSingleNode("//div[@id='verein_head']//h1[@itemprop='name']/span")?.InnerText;
            string imgUrl = doc.DocumentNode.SelectSingleNode("//div[@id='verein_head']//div[@class='dataBild ']/img")?.GetAttributeValue<string>("src", null);

            int? season = doc.DocumentNode.SelectSingleNode("//select[@name='saison_id']//option")?.GetAttributeValue<int>("value", 0);
            if (!season.HasValue)
            {
                season = 0;
            }

            HtmlNode countryNode = doc.DocumentNode.SelectSingleNode("//div[@id='verein_head']//span[@class='mediumpunkt']//img[@class='flaggenrahmen vm']");
            string country = countryNode?.GetAttributeValue<string>("title", null);
            string countryUrl = countryNode?.GetAttributeValue<string>("src", null);

            return (country, countryUrl, name, (int)season, imgUrl);
        }

        public DataTable GetCompetitionClubsTable()
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

        public DataTable GetClubSquadTable()
        {
            //Validate();

            string className = "items";

            HtmlNode table = doc.DocumentNode.SelectSingleNode("//table[@class='" + className + "']");

            if (table == null)
            {
                return null;
            }

            DataTable dataTable = new DataTable();
            //var headers = table.SelectNodes(".//thead/tr/th");
            var headers = table.SelectNodes(".//thead/tr[th]");
            var rows = table.SelectNodes(".//tbody/tr[td]");

            //foreach (HtmlNode header in h)
            //    dataTable.Columns.Add(header.InnerText);

            //foreach (var row in r)
            //    dataTable.Rows.Add(row.SelectNodes("td").Select(td => td.InnerText).ToArray());

            int? shirtNumberIndex = null;
            int? profileUrlIndex = null;
            int? nameIndex = null;
            int? shortNameIndex = null;
            int? imgUrlIndex = null;
            int? positionIndex = null;
            int? captainIndex = null;
            int? birthDateIndex = null;
            int? nationalityIndex = null;
            int? heightIndex = null;
            int? preferredFootIndex = null;
            int? clubArrivalDateIndex = null;
            int? contractExpirationDateIndex = null;
            int? marketValueIndex = null;

            if (headers != null && headers.Count > 0)
            {
                HtmlNodeCollection cols = headers[0].SelectNodes("th");

                for (int i = 0; i < cols.Count; i++)
                {
                    var col = cols[i].InnerText.Trim(' ', '\t', '\n');
                    if (col == "#")
                    {
                        shirtNumberIndex = i;
                    }
                    else if (col == "Jogadores")
                    {
                        profileUrlIndex = i;
                        nameIndex = i;
                        shortNameIndex = i;
                        imgUrlIndex = i;
                        positionIndex = i;
                        captainIndex = i;
                    }
                    else if (col == "Nasc. / idade")
                    {
                        birthDateIndex = i;
                    }
                    else if (col == "Nac.")
                    {
                        nationalityIndex = i;
                    }
                    else if (col == "Clube atual") { }
                    else if (col == "Altura")
                    {
                        heightIndex = i;
                    }
                    else if (col == "Pé")
                    {
                        preferredFootIndex = i;
                    }
                    else if (col == "Na equipa desde")
                    {
                        clubArrivalDateIndex = i;
                    }
                    else if (col == "Anterior") { }
                    else if (col == "Contrato até")
                    {
                        contractExpirationDateIndex = i;
                    }
                    else if (col == "Valor de mercado")
                    {
                        marketValueIndex = i;
                    }
                }
            }

            foreach (string enumName in Enum.GetNames(typeof(ClubColumnsEnum)))
            {
                dataTable.Columns.Add(enumName);
            }

            // each row is a player
            foreach (var row in rows)
            {
                //each column is an attribute
                HtmlNodeCollection cols = row.SelectNodes("td");

                string profileUrl = string.Empty;
                string shirtNumber = string.Empty;
                string name = string.Empty;
                string shortName = string.Empty;
                string imgUrl = string.Empty;
                string position = string.Empty;
                string captain = string.Empty;
                string nationality = string.Empty;
                string birthDate = string.Empty;
                string height = string.Empty;
                string preferredFoot = string.Empty;
                string clubArrivalDate = string.Empty;
                string contractExpirationDate = string.Empty;
                string marketValue = string.Empty;

                profileUrl = TryGetValue(GetProfileUrl, cols, profileUrlIndex.Value);
                shirtNumber = TryGetValue(GetShirtNumber, cols, shirtNumberIndex.Value);
                name = TryGetValue(GetName, cols, nameIndex.Value);
                shortName = TryGetValue(GetShortName, cols, shortNameIndex.Value);
                imgUrl = TryGetValue(GetImgUrl, cols, imgUrlIndex.Value);
                position = TryGetValue(GetPosition, cols, positionIndex.Value);
                captain = TryGetValue(GetCaptain, cols, captainIndex.Value);
                birthDate = TryGetValue(GetBirthDate, cols, birthDateIndex.Value);
                nationality = TryGetValue(GetNationality, cols, nationalityIndex.Value);
                height = TryGetValue(GetHeight, cols, heightIndex.Value);
                preferredFoot = TryGetValue(GetPreferredFoot, cols, preferredFootIndex.Value);
                clubArrivalDate = TryGetValue(GetClubArrivalDate, cols, clubArrivalDateIndex.Value);
                contractExpirationDate = TryGetValue(GetContractExpirationDate, cols, contractExpirationDateIndex.Value);
                marketValue = TryGetValue(GetMarketValue, cols, marketValueIndex.Value);

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

        #endregion Contract

        #region Competition Parsing

        private string GetClubUrl(HtmlNode node)
        {
            return node
                .SelectNodes("a")
                .FirstOrDefault(n => n.Attributes["class"]?.Value == "vereinprofil_tooltip")
                .Attributes["href"].Value;
        }

        #endregion Competition Parsing

        #region Squads Parsing

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
            var cap = node
                .SelectNodes("table//tr[1]/td[2]/span")?
                .FirstOrDefault(n => (n.Attributes["class"]?.Value).Contains("kapitaenicon-table"));
            return (cap == null) ? "" : "c";
        }
        private string GetNationality(HtmlNode node)
        {
            return node
                .SelectNodes("img")
                .Where(n => n.Attributes["class"]?.Value == "flaggenrahmen")
                .Select(n => n.Attributes["title"].Value)?.ToArray().FirstOrDefault();
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
            }
            else if (sp[1] == "mil")
            {
                n = n * 1000;
            }
            return n.ToString();
        }

        #endregion Squads Parsing

        #region Private methods

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

        private string TryGetValue(Func<HtmlNode, string> parserFunction, HtmlNodeCollection cols, int? columnIndex)
        {
            string value = null;
            try
            {
                if (columnIndex.HasValue)
                {
                    value = parserFunction(cols[columnIndex.Value]);
                }
            }
            catch (Exception ex)
            {
                //log
            }

            return value;
        }

        #endregion Private methods
    }

    static class HtmlAgilityPackUtil
    {
        public static T GetAttributeValue<T>(this HtmlNode td, string key, T defaultValue) where T : IConvertible
        {
            T result = defaultValue;//default(T);

            if (td.Attributes[key] == null)// || String.IsNullOrEmpty(td.Attributes[key].Value) == false)
            {
                return defaultValue;
            }

            string value = td.Attributes[key].Value;

            try
            {
                result = (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                result = defaultValue;//default(T);
            }

            return result;
        }
    }
}
