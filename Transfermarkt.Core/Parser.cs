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
        private readonly string dateFormat = "dd/MM/yyyy";

        public string BaseURL { get; } = ConfigurationManager.AppSettings["BaseURL"].ToString();
        public string SimpleClubUrlFormat { get; } = ConfigurationManager.AppSettings["SimpleClubUrlFormat"].ToString();
        public string PlusClubUrlFormat { get; } = ConfigurationManager.AppSettings["PlusClubUrlFormat"].ToString();
        public string IdentifiersGetterPattern { get; } = ConfigurationManager.AppSettings["IdentifiersGetterPattern"].ToString();
        public string IdentifiersSetterPattern { get; } = ConfigurationManager.AppSettings["IdentifiersSetterPattern"].ToString();

        public IPageConnector Connector { get; set; }
        public ConvertersCollection Converters { get; set; }

        public Parser(IPageConnector connector, ConvertersCollection converters)
        {
            Connector = connector;
            Converters = converters;
        }

        #region Contract

        public Competition ParseSquadsFromCompetition(string url)
        {
            Connector.ConnectToPage(url);

            (string country, string countryImg, string Name, int Season, string ImgUrl) = Connector.GetCompetitionData();

            Competition competition = new Competition
            {
                Name = Name,
                Country = Converters.NationalityConverter.Convert(country),
                CountryImg = countryImg,
                Season = Season,
                ImgUrl = ImgUrl
            };

            DataTable dt = Connector.GetCompetitionClubsTable();
            foreach (DataRow row in dt.Rows)
            {
                var clubUrl = row[CompetitionColumnsEnum.clubUrl.ToString()].ToString();

                string finalClubUrl = TransformUrl(clubUrl);

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
                Country = Converters.NationalityConverter.Convert(country),
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
                    Nationality = Converters.NationalityConverter.Convert(row[ClubColumnsEnum.nationality.ToString()].ToString()),
                    Height = i1,
                    PreferredFoot = Converters.FootConverter.Convert(row[ClubColumnsEnum.preferredFoot.ToString()].ToString()),
                    Position = Converters.PositionConverter.Convert(row[ClubColumnsEnum.position.ToString()].ToString()),
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
    }
}
