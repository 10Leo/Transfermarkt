using LJMB.Command;
using LJMB.Command.Commands;
using Transfermarkt.Console.Options;

namespace Transfermarkt.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Transfermarkt Web Scrapper\n");

            IContext context = new TMContext();
            var f = new FetchCommand(context);
            var p = new ParseCommand(context);
            _ = new ExitCommand(context);

            //TODO: register of options probablly makes more sense to be done inside each Command constructor because each command performs in its execution method
            //the logic of each option.
            f.RegisterOption(new YearOption());
            f.RegisterOption(new IndexesOption());
            p.RegisterOption(new YearOption());
            p.RegisterOption(new IndexesOption());
            context.Run();
        }
    }
}
