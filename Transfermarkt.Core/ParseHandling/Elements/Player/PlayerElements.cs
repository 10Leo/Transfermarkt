﻿using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.Player
{
    #region Person related

    public class Name : Element<string>
    {
        public Name() : base("Name", "Name")
        {
        }
    }

    public class ShortName : Element<string>
    {
        public ShortName() : base("ShortName", "Short Name")
        {
        }
    }

    public class BirthDate : Element<DateTime>
    {
        public BirthDate() : base("BirthDate", "Birth Date")
        {
        }
    }

    public class Nationality : Element<Actors.Nationality>
    {
        public Nationality() : base("Nat", "Nationality")
        {
        }
    }

    public class Height : Element<int>
    {
        public Height() : base("H", "Height")
        {
        }
    }

    #endregion Person related

    #region Football related

    public class PreferredFoot : Element<Actors.Foot>
    {
        public PreferredFoot() : base("Foot", "Preferred Foot")
        {
        }
    }

    public class Position : Element<Actors.Position>
    {
        public Position() : base("Pos", "Position")
        {
        }

    }

    #endregion Football related

    #region Club related

    public class ShirtNumber : Element<int>
    {
        public ShirtNumber() : base("#", "Shirt Number")
        {
        }
    }

    public class Captain : Element<int>
    {
        public Captain() : base("Cap", "Captain")
        {
        }
    }

    public class ClubArrivalDate : Element<DateTime>
    {
        public ClubArrivalDate() : base("ClubArrivalDate", "Club Arrival Date")
        {
        }
    }

    public class ContractExpirationDate : Element<DateTime>
    {
        public ContractExpirationDate() : base("ContractExpirationDate", "Contract Expiration Date")
        {
        }
    }

    #endregion Club related

    #region Business related

    public class MarketValue : Element<decimal>
    {
        public MarketValue() : base("MV", "Market Value")
        {
        }
    }

    #endregion Business related

    #region Links

    public class ImgUrl : Element<string>
    {
        public ImgUrl() : base("ImgUrl", "Img Url")
        {
        }
    }

    public class ProfileUrl : Element<string>
    {
        public ProfileUrl() : base("ProfileUrl", "Profile Url")
        {
        }
    }

    #endregion Links
}
