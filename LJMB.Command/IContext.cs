namespace LJMB.Command
{
    public interface IContext
    {
        bool Exit { get; set; }

        void RegisterCommand(ICommand command);
        void Run();
    }
}
