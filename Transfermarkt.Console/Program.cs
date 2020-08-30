using LJMB.Command;
using LJMB.Command.Commands;
using Transfermarkt.Console.Options;

namespace Transfermarkt.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            //TODO: create a progress marker to show how much was processed whenever a command is issued.
            System.Console.WriteLine("Transfermarkt Web Scrapper\n");

            IContext context = new TMContext();
            context.Run();
        }
    }
}
