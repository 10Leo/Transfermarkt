using LJMB.Command;
using LJMB.Command.Commands;

namespace Transfermarkt.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Transfermarkt Web Scrapper\n");

            IContext context = new Context();
            _ = new FetchCommand(context);
            _ = new ParseCommand(context);
            _ = new ExitCommand(context);
            context.Run();
        }
    }
}
