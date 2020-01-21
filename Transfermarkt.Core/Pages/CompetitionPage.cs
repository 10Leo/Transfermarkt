using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.Converters;
using Transfermarkt.Core.Parsers.HtmlAgilityPack.Competition;

namespace Transfermarkt.Core.Pages
{
    public class CompetitionPage : ICompetitionPage<HtmlNode>
    {
        public string BaseURL { get; } = ConfigurationManager.AppSettings["BaseURL"].ToString();
        public string SimpleClubUrlFormat { get; } = ConfigurationManager.AppSettings["SimpleClubUrlFormat"].ToString();
        public string PlusClubUrlFormat { get; } = ConfigurationManager.AppSettings["PlusClubUrlFormatV2"].ToString();
        public string IdentifiersGetterPattern { get; } = ConfigurationManager.AppSettings["IdentifiersGetterPattern"].ToString();
        public string IdentifiersSetterPattern { get; } = ConfigurationManager.AppSettings["IdentifiersSetterPattern"].ToString();

        private readonly string url;
        private HtmlDocument doc;

        public IDomain Domain { get; set; }

        public IElementParser<HtmlNode, int?> Season { get; set; }
        public IElementParser<HtmlNode, Nationality?> Country { get; set; }
        public IElementParser<HtmlNode, string> Name { get; set; }
        public IElementParser<HtmlNode, string> CountryImg { get; set; }
        public IElementParser<HtmlNode, string> ImgUrl { get; set; }
        public IClubPage<HtmlNode> Club { get; set; }

        public CompetitionPage(string url)
        {
            this.url = url;

            this.Domain = new Competition();

            this.Season = new SeasonParser();
            this.Season.Converter = new IntConverter();
            this.Season.OnSuccess += LogSuccess;
            this.Season.OnFailure += LogFailure;

            this.Country = new CountryParser();
            this.Country.Converter = new NationalityConverter();
            this.Country.OnSuccess += LogSuccess;
            this.Country.OnFailure += LogFailure;

            this.Name = new NameParser();
            this.Name.Converter = new StringConverter();
            this.Name.OnSuccess += LogSuccess;
            this.Name.OnFailure += LogFailure;

            this.CountryImg = new CountryImgParser();
            this.CountryImg.Converter = new StringConverter();
            this.CountryImg.OnSuccess += LogSuccess;
            this.CountryImg.OnFailure += LogFailure;

            this.ImgUrl = new ImgUrlParser();
            this.ImgUrl.Converter = new StringConverter();
            this.ImgUrl.OnSuccess += LogSuccess;
            this.ImgUrl.OnFailure += LogFailure;

            Connect();
        }

        public void Parse()
        {
            var competition = ((Competition)Domain);
            competition.Country = Country.Parse(doc.DocumentNode);
            competition.CountryImg = CountryImg.Parse(doc.DocumentNode);
            competition.ImgUrl = ImgUrl.Parse(doc.DocumentNode);
            competition.Name = Name.Parse(doc.DocumentNode);
            competition.Season = Season.Parse(doc.DocumentNode);


            HtmlNode table = doc.DocumentNode.SelectSingleNode("//div[@id='yw1']/table[@class='items']");
            if (table == null)
            {
                return;
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
                    string finalClubUrl = TransformUrl(clubUrl);

                    IClubPage<HtmlNode> page = new ClubPage($"{BaseURL}{finalClubUrl}");
                    page.Parse();

                    competition.Clubs.Add((Club)(((ClubPage)page).Domain));
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void Save()
        {
        }

        private void Connect()
        {
            //TODO: transform this in a service (generic) that connects to the page
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
                //Debug.WriteLine(ex.StackTrace);
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.StackTrace);
            }
        }

        private string GetClubUrl(HtmlNode node)
        {
            return node
                .SelectNodes("a")
                .FirstOrDefault(n => n.Attributes["class"]?.Value == "vereinprofil_tooltip")
                .Attributes["href"].Value;
        }

        private string TransformUrl(string url)
        {
            IList<string> identifiers = new List<string>();

            string simpleClubUrlPattern = SimpleClubUrlFormat;
            string finalClubUrl = PlusClubUrlFormat;

            MatchCollection ids = Regex.Matches(SimpleClubUrlFormat, IdentifiersGetterPattern);
            foreach (Match idMatch in ids)
            {
                identifiers.Add(idMatch.Groups[1].Value);
            }

            foreach (string identifier in identifiers)
            {
                simpleClubUrlPattern = simpleClubUrlPattern.Replace("{" + identifier + "}", IdentifiersSetterPattern.Replace("{ID}", identifier));
            }

            MatchCollection matches = Regex.Matches(url, simpleClubUrlPattern);
            if (!(matches.Count > 0 && matches[0].Groups.Count >= identifiers.Count))
            {
                //TODO: logging
            }

            for (int i = 1; i < matches[0].Groups.Count; i++)
            {
                Group group = matches[0].Groups[i];
                finalClubUrl = finalClubUrl.Replace("{" + group.Name + "}", group.Value);
            }

            return finalClubUrl;
        }

        private void LogSuccess(Object o, CustomEventArgs e)
        {
            Console.WriteLine(".");
        }

        private void LogFailure(Object o, CustomEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
