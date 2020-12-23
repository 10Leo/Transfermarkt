using LJMB.Command;
using LJMB.Command.Commands;
using Transfermarkt.Console.Options;

namespace Transfermarkt.Console
{
    public class Program
    {
        private static IContext context;

        static void Main(string[] args)
        {
            //TODO: create a progress marker to show how much was processed whenever a command is issued.
            System.Console.WriteLine("Transfermarkt Web Scrapper\n");

            context = new TMContext();
            context.GetCommands = () => Get();

            context.Run();
        }

        private static string GetInput()
        {
            System.Console.Write("> ");
            string input = System.Console.ReadLine();
            return input;
        }

        public static System.Collections.Generic.IEnumerable<string> Get()
        {
            while (!context.Exit)
            {
                yield return GetInput();
            }
        }
    }
}
