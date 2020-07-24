using Page.Scraper.Contracts;
using System;
using System.Globalization;
using Transfermarkt.Core.ParseHandling.Converters;

namespace Transfermarkt.Core.ParseHandling.Elements.Player
{
    #region Person related

    //TODO: use attributes to specify which field should be printed.
    //TODO: use res files to support different languages.
    public class Name : Element<StringValue, StringConverter>
    {
        public Name() : this(null) { }

        public Name(string value) : base("Name", "Name", value) { }
    }

    public class ShortName : Element<StringValue, StringConverter>
    {
        public ShortName() : this(null) { }

        public ShortName(string value) : base("ShortName", "Short Name", value) { }
    }

    public class BirthDate : Element<DatetimeValue, DateConverter>
    {
        public BirthDate() : this(null) { }

        public BirthDate(string value) : base("BirthDate", "Birth Date", value) { }

        public override string ToString()
        {
            return string.Format("[{0}: {1}]", InternalName, (Value.Value.HasValue ? Value.Value.Value.ToString(Common.date, CultureInfo.InvariantCulture) : ""));
        }
    }

    public class Nationality : Element<NationalityValue, NationalityConverter>
    {
        public Nationality() : this(null) { }

        public Nationality(string value) : base("Nat", "Nationality", value) { }
    }

    public class Height : Element<IntValue, IntConverter>
    {
        public Height() : this(null) { }

        public Height(string value) : base("H", "Height", value) { }
    }

    #endregion Person related

    #region Football related

    public class PreferredFoot : Element<FootValue, FootConverter>
    {
        public PreferredFoot() : this(null) { }

        public PreferredFoot(string value) : base("Foot", "Preferred Foot", value) { }
    }

    public class Position : Element<PositionValue, PositionConverter>
    {
        public Position() : this(null) { }

        public Position(string value) : base("Pos", "Position", value) { }
    }

    #endregion Football related

    #region Club related

    public class ShirtNumber : Element<IntValue, IntConverter>
    {
        public ShirtNumber() : this(null) { }

        public ShirtNumber(string value) : base("#", "Shirt Number", value) { }
    }

    public class Captain : Element<IntValue, IntConverter>
    {
        public Captain() : this(null) { }

        public Captain(string value) : base("Cap", "Captain", value) { }
    }

    public class ClubArrivalDate : Element<DatetimeValue, DateConverter>
    {
        public ClubArrivalDate() : this(null) { }

        public ClubArrivalDate(string value) : base("ClubArrivalDate", "Club Arrival Date", value) { }

        public override string ToString()
        {
            return string.Format("[{0}: {1}]", InternalName, (Value.Value.HasValue ? Value.Value.Value.ToString(Common.date) : ""));
        }
    }

    public class ContractExpirationDate : Element<DatetimeValue, DateConverter>
    {
        public ContractExpirationDate() : this(null) { }

        public ContractExpirationDate(string value) : base("ContractExpirationDate", "Contract Expiration Date", value) { }

        public override string ToString()
        {
            return string.Format("[{0}: {1}]", InternalName, (Value.Value.HasValue ? Value.Value.Value.ToString(Common.date) : ""));
        }
    }

    #endregion Club related

    #region Business related

    public class MarketValue : Element<DecimalValue, DecimalConverter>
    {
        public MarketValue() : this(null) { }

        public MarketValue(string value) : base("MV", "Market Value", value) { }
    }

    #endregion Business related

    #region Links

    public class ImgUrl : Element<StringValue, StringConverter>
    {
        public ImgUrl() : this(null) { }

        public ImgUrl(string value) : base("ImgUrl", "Img Url", value) { }
    }

    public class ProfileUrl : Element<StringValue, StringConverter>
    {
        public ProfileUrl() : this(null) { }

        public ProfileUrl(string value) : base("ProfileUrl", "Profile Url", value) { }
    }

    #endregion Links
}
