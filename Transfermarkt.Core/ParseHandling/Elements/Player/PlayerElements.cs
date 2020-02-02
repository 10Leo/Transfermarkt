using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.Player
{
    #region Person related

    class Name : Element
    {
        public Name() : base("Name", "Name")
        {
        }
    }

    class ShortName : Element
    {
        public ShortName() : base("ShortName", "Short Name")
        {
        }
    }

    class BirthDate : Element
    {
        public BirthDate() : base("BirthDate", "Birth Date")
        {
        }
    }

    class Nationality : Element
    {
        public Nationality() : base("Nat", "Nationality")
        {
        }
    }

    class Height : Element
    {
        public Height() : base("H", "Height")
        {
        }
    }

    #endregion Person related

    #region Football related

    class PreferredFoot : Element
    {
        public PreferredFoot() : base("Foot", "Preferred Foot")
        {
        }
    }

    class Position : Element
    {
        public Position() : base("Pos", "Position")
        {
        }

    }

    #endregion Football related

    #region Club related

    class ShirtNumber : Element
    {
        public ShirtNumber() : base("#", "Shirt Number")
        {
        }
    }

    class Captain : Element
    {
        public Captain() : base("Cap", "Captain")
        {
        }
    }

    class ClubArrivalDate : Element
    {
        public ClubArrivalDate() : base("ClubArrivalDate", "Club Arrival Date")
        {
        }
    }

    class ContractExpirationDate : Element
    {
        public ContractExpirationDate() : base("ContractExpirationDate", "Contract Expiration Date")
        {
        }
    }

    #endregion Club related

    #region Business related

    class MarketValue : Element
    {
        public MarketValue() : base("MV", "Market Value")
        {
        }
    }

    #endregion Business related

    #region Links

    class ImgUrl : Element
    {
        public ImgUrl() : base("ImgUrl", "Img Url")
        {
        }
    }

    class ProfileUrl : Element
    {
        public ProfileUrl() : base("ProfileUrl", "Profile Url")
        {
        }
    }

    #endregion Links
}
