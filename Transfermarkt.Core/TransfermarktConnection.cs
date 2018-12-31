using System;
using System.Collections.Generic;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using System.Configuration;
using System.Data;
using Transfermarkt.Core.Converters;

namespace Transfermarkt.Core
{
    public class TransfermarktConnection : IConnection
    {
        public string BaseURL { get; } = ConfigurationManager.AppSettings["BaseURL"].ToString();

        public IConnector Connector { get; set; }
        public INationalityConverter NationalityConverter { get; set; }
        public IPositionConverter PositionConverter { get; set; }
        public IFootConverter FootConverter { get; set; }

        public TransfermarktConnection(IConnector connector, INationalityConverter nationalityConverter, IPositionConverter positionConverter, IFootConverter footConverter)
        {
            Connector = connector;
            NationalityConverter = nationalityConverter;
            PositionConverter = positionConverter;
            FootConverter = footConverter;
        }

        public Competition ParseSquadsFromCompetition(string url)
        {
            throw new NotImplementedException();
        }

        public Club ParseSquad(string url)
        {
            Club club = new Club();
            Connector.ConnectToPage(url);

            DataTable dt = Connector.GetTableByClass("items");
            foreach (DataRow row in dt.Rows)
            {
                club.Squad.Add(new Player
                {
                    ProfileUrl = row[ColumnsEnum.profileUrl.ToString()].ToString(),
                    ImgUrl = row[ColumnsEnum.imgUrl.ToString()].ToString(),
                    Name = row[ColumnsEnum.name.ToString()].ToString(),
                    ShortName = row[ColumnsEnum.shortName.ToString()].ToString(),
                    BirthDate = DateTime.Parse(row[ColumnsEnum.birthDate.ToString()].ToString()),
                    Nationality = NationalityConverter.Convert(row[ColumnsEnum.nationality.ToString()].ToString()),
                    Height = int.Parse(row[ColumnsEnum.height.ToString()].ToString()),
                    PreferredFoot = FootConverter.Convert(row[ColumnsEnum.preferredFoot.ToString()].ToString()),
                    Position = PositionConverter.Convert(row[ColumnsEnum.position.ToString()].ToString()),
                    Number = int.Parse(row[ColumnsEnum.shirtNumber.ToString()].ToString()),
                    Captain = row[ColumnsEnum.captain.ToString()].ToString(),
                    ClubArrivalDate = DateTime.Parse(row[ColumnsEnum.clubArrivalDate.ToString()].ToString()),
                    ContractExpirationDate = DateTime.Parse(row[ColumnsEnum.contractExpirationDate.ToString()].ToString()),
                    MarketValue = decimal.Parse(row[ColumnsEnum.marketValue.ToString()].ToString())
                });
            }

            return club;
        }
    }
}
