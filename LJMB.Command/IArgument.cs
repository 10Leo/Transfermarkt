namespace LJMB.Command
{
    public interface IArgument { }

    public class StringArgument : IArgument
    {
        public string Value { get; set; }
    }
}
