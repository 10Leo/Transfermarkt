namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IPage<IDomain>
    {
        IDomain Domain { get; set; }

        void Parse();
        void Save();
    }
}
