namespace Transfermarkt.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Transfermarkt Web Scrapper\n");

            Context context = new Context();
            _ = new FetchCommand(context);
            _ = new ParseCommand(context);
            _ = new ExitCommand(context);
            context.Run();
        }
    }
}
