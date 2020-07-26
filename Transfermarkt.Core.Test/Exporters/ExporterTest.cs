using System;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Page.Scraper.Contracts;
using Transfermarkt.Core.Actors;
using Page.Scraper.Exporter;
using Transfermarkt.Core.ParseHandling;
using Transfermarkt.Core.ParseHandling.Contracts;
using Page.Scraper.Exporter.JSONExporter;
using System.Collections.Generic;
using LJMB.Common;

namespace Transfermarkt.Core.Test.Exporters
{
    [TestClass]
    public class ExporterTest
    {
        public static string ContinentFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ContinentFileNameFormat);
        public static string CompetitionFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.CompetitionFileNameFormat);
        public static string ClubFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ClubFileNameFormat);

        //public static string BaseFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseFolderPath);
        public static string OutputFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.OutputFolderPath);
        public static string Level1FolderFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.Level1FolderFormat);

        private IDictionary<string, IDomain> domains = new Dictionary<string, IDomain>
        {
            { "Europe", MockContinent(ContinentCode.EEE, "Europe") },
            { "Liga NOS", MockCompetition(Nationality.PRT, "Liga NOS", 2020, "http://NOS.pt", "http://NOS.pt") },
            { "Liga BBVA", MockCompetition(Nationality.ESP, "Liga BBVA", 2020, "http://BBVA.pt", "http://BBVA.pt") },
            { "Benfica", MockClub(Nationality.PRT, "Benfica", 2020, "http://Benfica.pt", "http://Benfica.pt") },
            { "Porto", MockClub(Nationality.PRT, "Porto", 2020, "http://Porto.pt", "http://Porto.pt") },
            { "Real Madrid", MockClub(Nationality.ESP, "Real Madrid", 2020, "http://Madrid.pt", "http://Madrid.pt") },
            { "Barcelona", MockClub(Nationality.ESP, "Barcelona", 2020, "http://Barcelona.pt", "http://Barcelona.pt") },
            { "Matic", MockPlayer("Nemanja Matic", "Matic", new DateTime(1985, 1, 1), Nationality.SRB, 191, Foot.L, Position.CM, 6, 0, new DateTime(2012, 1, 1), new DateTime(2016, 7, 1), 600000000, "http://link", "http://link") },
            { "Jonas", MockPlayer("Jonas", "Jonas", new DateTime(1981, 1, 1), Nationality.BRA, 182, Foot.R, Position.SS, 10, 0, new DateTime(2015, 7, 1), new DateTime(2020, 7, 1), 20000000, "http://link", "http://link") },
            { "Deco", MockPlayer("Deco", "Deco", new DateTime(1982, 1, 1), Nationality.BRA, 178, Foot.R, Position.AM, 10, 0, new DateTime(1999, 7, 1), new DateTime(2005, 7, 1), 6000000, "http://link", "http://link") },
            { "Ramos", MockPlayer("Sergio Ramos", "Ramos", new DateTime(1984, 1, 1), Nationality.ESP, 188, Foot.R, Position.CB, 4, 0, new DateTime(2006, 7, 1), null, 70000000, "http://link", "http://link") },
            { "Messi", MockPlayer("Lio Messi", "Messi", new DateTime(1986, 1, 1), Nationality.ARG, 171, Foot.L, Position.AM, 10, 1, new DateTime(2003, 7, 1), new DateTime(2022, 7, 1), 200000000, "http://link", "http://link") },
            { "Busquets", MockPlayer("Sergio Busquets", "Busquets", new DateTime(1986, 1, 1), Nationality.ESP, 184, Foot.R, Position.DM, 6, 0, new DateTime(2008, 7, 1), null, 70000000, "http://link", "http://link") }
        };

        [TestMethod, TestCategory("Export")]
        public void SuccessfullyExportsAClubAsJSON()
        {
            IExporter exporter = new JsonExporter(OutputFolderPath, Level1FolderFormat);

            IDomain club = domains["Benfica"];

            IDomain player01 = domains["Matic"];
            IDomain player02 = domains["Jonas"];

            club.Children.Add(player01);
            club.Children.Add(player02);

            exporter.Extract(club, ClubFileNameFormat);
        }

        [TestMethod, TestCategory("Export")]
        public void SuccessfullyExportsAClubAsJSONWhoseConfigContainsInvalidElementName()
        {
            ConfigurationManager.AppSettings[Keys.Config.ClubFileNameFormat] = "{COUNTRY}-{NAME}_{Y}_{IMGURL}_{NOT}";

            IExporter exporter = new JsonExporter(OutputFolderPath, Level1FolderFormat);

            IDomain club = domains["Benfica"];

            IDomain player01 = domains["Matic"];
            IDomain player02 = domains["Jonas"];

            club.Children.Add(player01);
            club.Children.Add(player02);

            exporter.Extract(club, ClubFileNameFormat);
        }

        [TestMethod, TestCategory("Export")]
        public void SuccessfullyExportsACompetitionAsJSON()
        {
            IExporter exporter = new JsonExporter(OutputFolderPath, Level1FolderFormat);

            IDomain competition = domains["Liga NOS"];

            IDomain club01 = domains["Benfica"];
            IDomain club02 = domains["Porto"];

            IDomain player011 = domains["Matic"];
            IDomain player012 = domains["Jonas"];

            IDomain player021 = domains["Deco"];

            competition.Children.Add(club01);
            competition.Children.Add(club02);

            club01.Children.Add(player011);
            club01.Children.Add(player012);

            club02.Children.Add(player021);

            exporter.Extract(competition, CompetitionFileNameFormat);
        }

        [TestMethod, TestCategory("Export")]
        public void SuccessfullyExportsAContinentAsJSON()
        {
            IExporter exporter = new JsonExporter(OutputFolderPath, Level1FolderFormat);

            IDomain continent = domains["Europe"];

            IDomain competition01 = domains["Liga NOS"];
            IDomain competition02 = domains["Liga BBVA"];

            IDomain club01 = domains["Benfica"];
            IDomain club02 = domains["Porto"];

            IDomain club11 = domains["Real Madrid"];
            IDomain club12 = domains["Barcelona"];


            IDomain player011 = domains["Matic"];
            IDomain player012 = domains["Jonas"];

            IDomain player021 = domains["Deco"];

            IDomain player111 = domains["Ramos"];

            IDomain player121 = domains["Busquets"];
            IDomain player122 = domains["Messi"];

            continent.Children.Add(competition01);
            continent.Children.Add(competition02);

            competition01.Children.Add(club01);
            competition01.Children.Add(club02);
            competition02.Children.Add(club11);
            competition02.Children.Add(club12);

            club01.Children.Add(player011);
            club01.Children.Add(player012);

            club02.Children.Add(player021);

            club11.Children.Add(player111);

            club12.Children.Add(player121);
            club12.Children.Add(player122);

            exporter.Extract(continent, ContinentFileNameFormat);
        }

        private static Continent MockContinent(ContinentCode continentCode, string name)
        {
            Continent domain = new Continent();

            ((Core.ParseHandling.Elements.Continent.ContinentCode)domain.Elements.FirstOrDefault(e => e.InternalName == "Code")).Value = new ContinentCodeValue { Value = continentCode };
            ((Core.ParseHandling.Elements.Continent.Name)domain.Elements.FirstOrDefault(e => e.InternalName == "Name")).Value = new StringValue { Value = name };

            return domain;
        }

        private static Competition MockCompetition(Nationality nationality, string name, int season, string imgUrl, string countryImg)
        {
            Competition domain = new Competition();

            ((Core.ParseHandling.Elements.Competition.Country)domain.Elements.FirstOrDefault(e => e.InternalName == "Country")).Value = new NationalityValue { Value = nationality };
            ((Core.ParseHandling.Elements.Competition.Name)domain.Elements.FirstOrDefault(e => e.InternalName == "Name")).Value = new StringValue { Value = name };
            ((Core.ParseHandling.Elements.Competition.Season)domain.Elements.FirstOrDefault(e => e.InternalName == "Y")).Value = new IntValue { Value = season };
            ((Core.ParseHandling.Elements.Competition.ImgUrl)domain.Elements.FirstOrDefault(e => e.InternalName == "ImgUrl")).Value = new StringValue { Value = imgUrl };
            ((Core.ParseHandling.Elements.Competition.CountryImg)domain.Elements.FirstOrDefault(e => e.InternalName == "CountryImg")).Value = new StringValue { Value = countryImg };

            return domain;
        }

        private static Club MockClub(Nationality nationality, string name, int season, string imgUrl, string countryImg)
        {
            Club domain = new Club();

            ((Core.ParseHandling.Elements.Club.Country)domain.Elements.FirstOrDefault(e => e.InternalName == "Country")).Value = new NationalityValue { Value = nationality };
            ((Core.ParseHandling.Elements.Club.Name)domain.Elements.FirstOrDefault(e => e.InternalName == "Name")).Value = new StringValue { Value = name };
            ((Core.ParseHandling.Elements.Club.Season)domain.Elements.FirstOrDefault(e => e.InternalName == "Y")).Value = new IntValue { Value = season };
            ((Core.ParseHandling.Elements.Club.ImgUrl)domain.Elements.FirstOrDefault(e => e.InternalName == "ImgUrl")).Value = new StringValue { Value = imgUrl };
            ((Core.ParseHandling.Elements.Club.CountryImg)domain.Elements.FirstOrDefault(e => e.InternalName == "CountryImg")).Value = new StringValue { Value = countryImg };

            return domain;
        }

        private static Player MockPlayer(string name, string shortName, DateTime? birthDate, Nationality? nat, int? h, Foot foot, Position pos, int? number, int cap, DateTime? clubArrivalDate, DateTime? contractExpirationDate, decimal? mv, string imgUrl, string profileUrl)
        {
            Player domain = new Player();

            ((Core.ParseHandling.Elements.Player.BirthDate)domain.Elements.FirstOrDefault(e => e.InternalName == "BirthDate")).Value = new DatetimeValue { Value = birthDate };
            ((Core.ParseHandling.Elements.Player.Captain)domain.Elements.FirstOrDefault(e => e.InternalName == "Cap")).Value = new IntValue { Value = cap };
            ((Core.ParseHandling.Elements.Player.ClubArrivalDate)domain.Elements.FirstOrDefault(e => e.InternalName == "ClubArrivalDate")).Value = new DatetimeValue { Value = clubArrivalDate };
            ((Core.ParseHandling.Elements.Player.ContractExpirationDate)domain.Elements.FirstOrDefault(e => e.InternalName == "ContractExpirationDate")).Value = new DatetimeValue { Value = contractExpirationDate };
            ((Core.ParseHandling.Elements.Player.Height)domain.Elements.FirstOrDefault(e => e.InternalName == "H")).Value = new IntValue { Value = h };
            ((Core.ParseHandling.Elements.Player.ImgUrl)domain.Elements.FirstOrDefault(e => e.InternalName == "ImgUrl")).Value = new StringValue { Value = imgUrl };
            ((Core.ParseHandling.Elements.Player.MarketValue)domain.Elements.FirstOrDefault(e => e.InternalName == "MV")).Value = new DecimalValue { Value = mv };
            ((Core.ParseHandling.Elements.Player.Name)domain.Elements.FirstOrDefault(e => e.InternalName == "Name")).Value = new StringValue { Value = name };
            ((Core.ParseHandling.Elements.Player.Nationality)domain.Elements.FirstOrDefault(e => e.InternalName == "Nat")).Value = new NationalityValue { Value = nat };
            ((Core.ParseHandling.Elements.Player.Position)domain.Elements.FirstOrDefault(e => e.InternalName == "Pos")).Value = new PositionValue { Value = pos };
            ((Core.ParseHandling.Elements.Player.PreferredFoot)domain.Elements.FirstOrDefault(e => e.InternalName == "Foot")).Value = new FootValue { Value = foot };
            ((Core.ParseHandling.Elements.Player.ProfileUrl)domain.Elements.FirstOrDefault(e => e.InternalName == "ProfileUrl")).Value = new StringValue { Value = profileUrl };
            ((Core.ParseHandling.Elements.Player.ShirtNumber)domain.Elements.FirstOrDefault(e => e.InternalName == "#")).Value = new IntValue { Value = number };
            ((Core.ParseHandling.Elements.Player.ShortName)domain.Elements.FirstOrDefault(e => e.InternalName == "ShortName")).Value = new StringValue { Value = shortName };

            return domain;
        }
    }
}
