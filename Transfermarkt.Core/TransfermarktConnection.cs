using System;
using System.Collections.Generic;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using System.Configuration;
using System.Data;

namespace Transfermarkt.Core
{
    public class TransfermarktConnection : IConnection
    {
        public string BaseURL { get; } = ConfigurationManager.AppSettings["BaseURL"].ToString();

        public IConnector Connector { get; set; }

        public TransfermarktConnection(IConnector connector)
        {
            Connector = connector;
        }

        public IList<Competition> GetCompetitions(int season, Nationality? nationality = null)
        {
            throw new NotImplementedException();
        }

        public IList<Club> GetClubs()
        {
            throw new NotImplementedException();
        }

        public IList<Player> GetPlayers(int season, Nationality? nationality = null)
        {
            IList<Player> players = new List<Player>();

            return players;
        }

        public Player ParsePlayer(string url)
        {
            Player player = new Player();


            return player;
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
                    Name = row[ColumnsEnum.name.ToString()].ToString(),
                    Number = int.Parse(row[ColumnsEnum.shirtNumber.ToString()].ToString()),
                    BirthDate = DateTime.Parse(row[ColumnsEnum.birthDate.ToString()].ToString()),
                    Captain = row[ColumnsEnum.captain.ToString()].ToString(),
                    Height = int.Parse(row[ColumnsEnum.height.ToString()].ToString()),
                    //Nationality = (Nationality)Enum.Parse(typeof(Nationality), row[ColumnsEnum.nationality.ToString()].ToString()),
                    Nationality = row[ColumnsEnum.nationality.ToString()].ToString(),
                    PreferredFoot = row[ColumnsEnum.preferredFoot.ToString()].ToString(),
                    ClubArrivalDate = DateTime.Parse(row[ColumnsEnum.clubArrivalDate.ToString()].ToString()),
                    ContractExpirationDate = DateTime.Parse(row[ColumnsEnum.contractExpirationDate.ToString()].ToString()),
                    MarketValue = int.Parse(row[ColumnsEnum.marketValue.ToString()].ToString())
                });
            }

            return club;
        }

        public Competition ParseSquadsFromCompetition(string url)
        {
            throw new NotImplementedException();
        }
    }
}
