using System;
using System.Collections.Generic;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using System.Configuration;
using System.Data;
using Transfermarkt.Core.Converters;
using System.Globalization;
using System.Text.RegularExpressions;
using Transfermarkt.Core.Contracts.Converters;

namespace Transfermarkt.Core
{
    public class Parser : IParser
    {
        private const string CLUB_STRING = "CLUB_STRING";
        private const string CLUB_ID = "CLUB_ID";
        private const string SEASON = "SEASON";
        private readonly string dateFormat = "dd/MM/yyyy";

        //private readonly string simpleClubUrlFormat = "/{CLUB_STRING}/startseite/verein/{CLUB_ID}/saison_id/{SEASON}";
        private readonly string plusClubUrlPattern = "/{1}/kader/verein/{2}/saison_id/{3}/plus/1";
        private readonly string simpleClubUrlPattern = $"/(?<{CLUB_STRING}>.*)/startseite/verein/(?<{CLUB_ID}>.*)/saison_id/(?<{SEASON}>.*)";

        public string BaseURL { get; } = ConfigurationManager.AppSettings["BaseURL"].ToString();

        public IPageConnector Connector { get; set; }
        public INationalityConverter NationalityConverter { get; set; }
        public IPositionConverter PositionConverter { get; set; }
        public IFootConverter FootConverter { get; set; }

        public Parser(IPageConnector connector, INationalityConverter nationalityConverter, IPositionConverter positionConverter, IFootConverter footConverter)
        {
            Connector = connector;
            NationalityConverter = nationalityConverter;
            PositionConverter = positionConverter;
            FootConverter = footConverter;

            //string f = "(?<ID>.*)";
        }

        #region Contract

        public Competition ParseSquadsFromCompetition(string url)
        {
            Connector.ConnectToPage(url);

            (string country, string countryImg, string Name, int Season, string ImgUrl) = Connector.GetCompetitionData();

            Competition competition = new Competition
            {
                Name = Name,
                Country = NationalityConverter.Convert(country),
                CountryImg = countryImg,
                Season = Season,
                ImgUrl = ImgUrl
            };

            DataTable dt = Connector.GetCompetitionClubsTable();
            foreach (DataRow row in dt.Rows)
            {
                var clubUrl = row[CompetitionColumnsEnum.clubUrl.ToString()].ToString();

                string finalClubUrl = plusClubUrlPattern;
                MatchCollection matches = Regex.Matches(clubUrl, simpleClubUrlPattern);
                if (!(matches.Count > 0 && matches[0].Groups.Count >= 3))
                {
                    //TODO: logging
                }

                finalClubUrl = finalClubUrl.Replace("{1}", matches[0].Groups[CLUB_STRING].Value);
                finalClubUrl = finalClubUrl.Replace("{2}", matches[0].Groups[CLUB_ID].Value);
                finalClubUrl = finalClubUrl.Replace("{3}", matches[0].Groups[SEASON].Value);

                competition.Clubs.Add(ParseSquad($"{BaseURL}{finalClubUrl}"));
            }

            return competition;
        }

        public Club ParseSquad(string url)
        {
            Connector.ConnectToPage(url);

            (string country, string countryImg, string Name, int Season, string ImgUrl) = Connector.GetClubData();

            Club club = new Club
            {
                Name = Name,
                Country = NationalityConverter.Convert(country),
                CountryImg = countryImg,
                Season = Season,
                ImgUrl = ImgUrl
            };

            DataTable dt = Connector.GetClubSquadTable();
            foreach (DataRow row in dt.Rows)
            {
                DateTime? d1 = null, d2 = null, d3 = null;
                int i1 = 0, i2 = 0;
                decimal i3 = 0;
                try
                {
                    d1 = DateTime.ParseExact(row[ClubColumnsEnum.birthDate.ToString()].ToString(), dateFormat, CultureInfo.InvariantCulture);
                }
                catch (Exception) { }
                try
                {
                    d2 = DateTime.ParseExact(row[ClubColumnsEnum.clubArrivalDate.ToString()].ToString(), dateFormat, CultureInfo.InvariantCulture);
                }
                catch (Exception) { }
                try
                {
                    d3 = DateTime.ParseExact(row[ClubColumnsEnum.contractExpirationDate.ToString()].ToString(), dateFormat, CultureInfo.InvariantCulture);
                }
                catch (Exception) { }

                try
                {
                    i1 = int.Parse(row[ClubColumnsEnum.height.ToString()].ToString());
                }
                catch (Exception) { }
                try
                {
                    i2 = int.Parse(row[ClubColumnsEnum.shirtNumber.ToString()].ToString());
                }
                catch (Exception) { }
                try
                {
                    i3 = decimal.Parse(row[ClubColumnsEnum.marketValue.ToString()].ToString());
                }
                catch (Exception) { }

                club.Squad.Add(new Player
                {
                    ProfileUrl = row[ClubColumnsEnum.profileUrl.ToString()].ToString(),
                    ImgUrl = row[ClubColumnsEnum.imgUrl.ToString()].ToString(),
                    Name = row[ClubColumnsEnum.name.ToString()].ToString(),
                    ShortName = row[ClubColumnsEnum.shortName.ToString()].ToString(),
                    BirthDate = d1,
                    Nationality = NationalityConverter.Convert(row[ClubColumnsEnum.nationality.ToString()].ToString()),
                    Height = i1,
                    PreferredFoot = FootConverter.Convert(row[ClubColumnsEnum.preferredFoot.ToString()].ToString()),
                    Position = PositionConverter.Convert(row[ClubColumnsEnum.position.ToString()].ToString()),
                    Number = i2,
                    Captain = row[ClubColumnsEnum.captain.ToString()].ToString(),
                    ClubArrivalDate = d2,
                    ContractExpirationDate = d3,
                    MarketValue = i3
                });
            }

            return club;
        }

        #endregion Contract
    }
}
