namespace LJMB.Command
{
    public interface IArgument { }

    public class StringParameterArgument : IArgument
    {
        public string Value { get; set; }
    }
}
