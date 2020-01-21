using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.Contracts.Parsers;
using Transfermarkt.Core.Converters;
using Transfermarkt.Core.Parsers.HtmlAgilityPack;
using Transfermarkt.Core.Parsers.HtmlAgilityPack.Club;
using Transfermarkt.Core.Parsers.HtmlAgilityPack.Player;

namespace Transfermarkt.Core.Pages
{
    public class ClubPage : IClubPage<HtmlNode>
    {
        private readonly string url;
        private HtmlDocument doc;

        public IDomain Domain { get; set; }

        public IElementParser<HtmlNode, int?> Season { get; set; }
        public IElementParser<HtmlNode, Nationality?> Country { get; set; }
        public IElementParser<HtmlNode, string> Name { get; set; }
        public IElementParser<HtmlNode, string> CountryImg { get; set; }
        public IElementParser<HtmlNode, string> ImgUrl { get; set; }

        //Player
        public IElementParser<HtmlNode, string> ProfileUrl { get; set; }
        public IElementParser<HtmlNode, int?> ShirtNumber { get; set; }
        public IElementParser<HtmlNode, string> PlayerName { get; set; }
        public IElementParser<HtmlNode, string> ShortName { get; set; }
        public IElementParser<HtmlNode, string> PlayerImgUrl { get; set; }
        public IElementParser<HtmlNode, Position?> Position { get; set; }
        public IElementParser<HtmlNode, int?> Captain { get; set; }
        public IElementParser<HtmlNode, DateTime?> BirthDate { get; set; }
        public INationalityParser<HtmlNode> Nationality { get; set; }
        public IElementParser<HtmlNode, int?> Height { get; set; }
        public IElementParser<HtmlNode, Foot?> PreferredFoot { get; set; }
        public IElementParser<HtmlNode, DateTime?> ClubArrivalDate { get; set; }
        public IElementParser<HtmlNode, DateTime?> ContractExpirationDate { get; set; }
        public IMarketValueParser<HtmlNode> MarketValue { get; set; }

        public ClubPage(string url)
        {
            this.url = url;

            this.Domain = new Club();

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

            //Player
            this.ProfileUrl = new ProfileUrlParser();
            this.ProfileUrl.Converter = new StringConverter();
            this.ProfileUrl.OnSuccess += LogSuccess;
            this.ProfileUrl.OnFailure += LogFailure;

            this.ShirtNumber = new ShirtNumberParser();
            this.ShirtNumber.Converter = new IntConverter();
            this.ShirtNumber.OnSuccess += LogSuccess;
            this.ShirtNumber.OnFailure += LogFailure;

            this.PlayerName = new PlayerNameParser();
            this.PlayerName.Converter = new StringConverter();
            this.PlayerName.OnSuccess += LogSuccess;
            this.PlayerName.OnFailure += LogFailure;

            this.ShortName = new ShortNameParser();
            this.ShortName.Converter = new StringConverter();
            this.ShortName.OnSuccess += LogSuccess;
            this.ShortName.OnFailure += LogFailure;

            this.PlayerImgUrl = new PlayerImgUrlParser();
            this.PlayerImgUrl.Converter = new StringConverter();
            this.PlayerImgUrl.OnSuccess += LogSuccess;
            this.PlayerImgUrl.OnFailure += LogFailure;

            this.Position = new PositionParser();
            this.Position.Converter = new PositionConverter();
            this.Position.OnSuccess += LogSuccess;
            this.Position.OnFailure += LogFailure;

            this.Captain = new CaptainParser();
            this.Captain.Converter = new IntConverter();
            this.Captain.OnSuccess += LogSuccess;
            this.Captain.OnFailure += LogFailure;

            this.BirthDate = new BirthDateParser();
            this.BirthDate.Converter = new DateConverter();
            this.BirthDate.OnSuccess += LogSuccess;
            this.BirthDate.OnFailure += LogFailure;

            this.Nationality = new NationalityParser();
            //TODO: change converter initializer to instantiate according to the language defined on config. Like this is tied to the PT one.
            this.Nationality.Converter = new NationalityConverter();
            this.Nationality.OnSuccess += LogSuccess;
            this.Nationality.OnFailure += LogFailure;

            this.Height = new HeightParser();
            this.Height.Converter = new IntConverter();
            this.Height.OnSuccess += LogSuccess;
            this.Height.OnFailure += LogFailure;

            this.PreferredFoot = new PreferredFootParser();
            this.PreferredFoot.Converter = new FootConverter();
            this.PreferredFoot.OnSuccess += LogSuccess;
            this.PreferredFoot.OnFailure += LogFailure;

            this.ClubArrivalDate = new ClubArrivalDateParser();
            this.ClubArrivalDate.Converter = new DateConverter();
            this.ClubArrivalDate.OnSuccess += LogSuccess;
            this.ClubArrivalDate.OnFailure += LogFailure;

            this.ContractExpirationDate = new ContractExpirationDateParser();
            this.ContractExpirationDate.Converter = new DateConverter();
            this.ContractExpirationDate.OnSuccess += LogSuccess;
            this.ContractExpirationDate.OnFailure += LogFailure;

            this.MarketValue = new MarketValueParser();
            this.MarketValue.Converter = new DecimalConverter();
            this.MarketValue.OnSuccess += LogSuccess;
            this.MarketValue.OnFailure += LogFailure;

            Connect();
        }

        #region Contract

        public void Parse()
        {
            var club = (Club)Domain;
            club.Country = Country.Parse(doc.DocumentNode);
            club.CountryImg = CountryImg.Parse(doc.DocumentNode);
            club.ImgUrl = ImgUrl.Parse(doc.DocumentNode);
            club.Name = Name.Parse(doc.DocumentNode);
            club.Season = Season.Parse(doc.DocumentNode);


            HtmlNode table = GetTable();
            if (table == null)
            {
                return;
            }

            var headers = table.SelectNodes(".//thead/tr[th]");
            var rows = table.SelectNodes(".//tbody/tr[td]");
            HtmlNodeCollection headerCols = headers[0].SelectNodes("th");

            //each row is a player
            foreach (var row in rows)
            {
                Player player = new Player();
                club.Squad.Add(player);

                //each column is an attribute
                HtmlNodeCollection cols = row.SelectNodes("td");

                for (int i = 0; i < cols.Count; i++)
                {
                    var header = headerCols[i];
                    var element = cols[i];

                    if (ProfileUrl.CanParse(header))
                    {
                        player.ProfileUrl = ProfileUrl.Parse(element);
                    }
                    if (ShirtNumber.CanParse(header))
                    {
                        player.Number = ShirtNumber.Parse(element);
                    }
                    if (PlayerName.CanParse(header))
                    {
                        player.Name = PlayerName.Parse(element);
                    }
                    if (ShortName.CanParse(header))
                    {
                        player.ShortName = ShortName.Parse(element);
                    }
                    if (PlayerImgUrl.CanParse(header))
                    {
                        player.ImgUrl = PlayerImgUrl.Parse(element);
                    }
                    if (Position.CanParse(header))
                    {
                        player.Position = Position.Parse(element);
                    }
                    if (Captain.CanParse(header))
                    {
                        player.Captain = Captain.Parse(element);
                    }
                    if (BirthDate.CanParse(header))
                    {
                        player.BirthDate = BirthDate.Parse(element);
                    }
                    if (Nationality.CanParse(header))
                    {
                        player.Nationality = Nationality.Parse(element);
                    }
                    if (Height.CanParse(header))
                    {
                        player.Height = Height.Parse(element);
                    }
                    if (PreferredFoot.CanParse(header))
                    {
                        player.PreferredFoot = PreferredFoot.Parse(element);
                    }
                    if (ClubArrivalDate.CanParse(header))
                    {
                        player.ClubArrivalDate = ClubArrivalDate.Parse(element);
                    }
                    if (ContractExpirationDate.CanParse(header))
                    {
                        player.ContractExpirationDate = ContractExpirationDate.Parse(element);
                    }
                    if (MarketValue.CanParse(header))
                    {
                        player.MarketValue = MarketValue.Parse(element);
                    }
                }
            }
        }

        public void Save()
        {
        }

        #endregion

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

        private HtmlNode GetTable()
        {
            return doc.DocumentNode.SelectSingleNode("//table[@class='items']");
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
