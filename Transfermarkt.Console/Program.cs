using LJMB.Command;
using LJMB.Command.Commands;
using LJMB.Common;
using Transfermarkt.Console.Options;
using Transfermarkt.Core;

namespace Transfermarkt.Console
{
    public class Program
    {
        private static string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);
        public static string ContinentFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ContinentFileNameFormat);
        public static string CompetitionFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.CompetitionFileNameFormat);
        public static string ClubFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ClubFileNameFormat);

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
