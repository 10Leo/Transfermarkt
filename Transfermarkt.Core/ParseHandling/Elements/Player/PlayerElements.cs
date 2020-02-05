using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.Player
{
    #region Person related

    public class Name : Element
    {
        public Name() : base("Name", "Name")
        {
        }
    }

    public class ShortName : Element
    {
        public ShortName() : base("ShortName", "Short Name")
        {
        }
    }

    public class BirthDate : Element
    {
        public BirthDate() : base("BirthDate", "Birth Date")
        {
        }
    }

    public class Nationality : Element
    {
        public Nationality() : base("Nat", "Nationality")
        {
        }
    }

    public class Height : Element
    {
        public Height() : base("H", "Height")
        {
        }
    }

    #endregion Person related

    #region Football related

    public class PreferredFoot : Element
    {
        public PreferredFoot() : base("Foot", "Preferred Foot")
        {
        }
    }

    public class Position : Element
    {
        public Position() : base("Pos", "Position")
        {
        }

    }

    #endregion Football related

    #region Club related

    public class ShirtNumber : Element
    {
        public ShirtNumber() : base("#", "Shirt Number")
        {
        }
    }

    public class Captain : Element
    {
        public Captain() : base("Cap", "Captain")
        {
        }
    }

    public class ClubArrivalDate : Element
    {
        public ClubArrivalDate() : base("ClubArrivalDate", "Club Arrival Date")
        {
        }
    }

    public class ContractExpirationDate : Element
    {
        public ContractExpirationDate() : base("ContractExpirationDate", "Contract Expiration Date")
        {
        }
    }

    #endregion Club related

    #region Business related

    public class MarketValue : Element
    {
        public MarketValue() : base("MV", "Market Value")
        {
        }
    }

    #endregion Business related

    #region Links

    public class ImgUrl : Element
    {
        public ImgUrl() : base("ImgUrl", "Img Url")
        {
        }
    }

    public class ProfileUrl : Element
    {
        public ProfileUrl() : base("ProfileUrl", "Profile Url")
        {
        }
    }

    #endregion Links
}
