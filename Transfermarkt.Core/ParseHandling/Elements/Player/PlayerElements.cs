using Page.Parser.Contracts;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.Player
{
    #region Person related

    //TODO: use attributes to specify which field should be printed.
    public class Name : Element<StringValue>
    {
        public Name() : base("Name", "Name")
        {
            this.Value = new StringValue();
        }
        public Name(string value) : base("Name", "Name")
        {
            this.Value = new StringValue { Value = value };
        }
    }

    public class ShortName : Element<StringValue>
    {
        public ShortName() : base("ShortName", "Short Name")
        {
            this.Value = new StringValue();
        }
    }

    public class BirthDate : Element<DatetimeValue>
    {
        public BirthDate() : base("BirthDate", "Birth Date")
        {
            this.Value = new DatetimeValue { };
        }

        public override string ToString()
        {
            return string.Format("[{0}: {1}]", InternalName, (Value.Value.HasValue ? Value.Value.Value.ToString(Common.date) : ""));
        }
    }

    public class Nationality : Element<NationalityValue>
    {
        public Nationality() : base("Nat", "Nationality")
        {
            this.Value = new NationalityValue();
        }
    }

    public class Height : Element<IntValue>
    {
        public Height() : base("H", "Height")
        {
            this.Value = new IntValue();
        }
    }

    #endregion Person related

    #region Football related

    public class PreferredFoot : Element<FootValue>
    {
        public PreferredFoot() : base("Foot", "Preferred Foot")
        {
            this.Value = new FootValue();
        }
    }

    public class Position : Element<PositionValue>
    {
        public Position() : base("Pos", "Position")
        {
            this.Value = new PositionValue();
        }
    }

    #endregion Football related

    #region Club related

    public class ShirtNumber : Element<IntValue>
    {
        public ShirtNumber() : base("#", "Shirt Number")
        {
            this.Value = new IntValue();
        }
    }

    public class Captain : Element<IntValue>
    {
        public Captain() : base("Cap", "Captain")
        {
            this.Value = new IntValue();
        }
    }

    public class ClubArrivalDate : Element<DatetimeValue>
    {
        public ClubArrivalDate() : base("ClubArrivalDate", "Club Arrival Date")
        {
            this.Value = new DatetimeValue();
        }

        public override string ToString()
        {
            return string.Format("[{0}: {1}]", InternalName, (Value.Value.HasValue ? Value.Value.Value.ToString(Common.date) : ""));
        }
    }

    public class ContractExpirationDate : Element<DatetimeValue>
    {
        public ContractExpirationDate() : base("ContractExpirationDate", "Contract Expiration Date") {
            this.Value = new DatetimeValue();
        }

        public ContractExpirationDate(DateTime? value) : base("ContractExpirationDate", "Contract Expiration Date")
        {
            this.Value = new DatetimeValue { Value = value };
        }

        public override string ToString()
        {
            return string.Format("[{0}: {1}]", InternalName, (Value.Value.HasValue ? Value.Value.Value.ToString(Common.date) : ""));
        }
    }

    #endregion Club related

    #region Business related

    public class MarketValue : Element<DecimalValue>
    {
        public MarketValue() : base("MV", "Market Value")
        {
            this.Value = new DecimalValue();
        }
    }

    #endregion Business related

    #region Links

    public class ImgUrl : Element<StringValue>
    {
        public ImgUrl() : base("ImgUrl", "Img Url")
        {
            this.Value = new StringValue();
        }
    }

    public class ProfileUrl : Element<StringValue>
    {
        public ProfileUrl() : base("ProfileUrl", "Profile Url")
        {
            this.Value = new StringValue();
        }
    }

    #endregion Links
}
