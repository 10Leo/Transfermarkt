using LJMB.Command;
using LJMB.Common;
using LJMB.Logging;
using Page.Scraper.Exporter;
using Page.Scraper.Exporter.JSONExporter;
using System.Collections.Generic;
using Transfermarkt.Core;
using Transfermarkt.Core.Service;

namespace Transfermarkt.Console
{
    public class Program
    {
        protected static string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);
        protected static string ContinentFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ContinentFileNameFormat);
        protected static string CompetitionFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.CompetitionFileNameFormat);
        protected static string ClubFileNameFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.ClubFileNameFormat);

        protected static int MinimumLoggingLevel { get; } = ConfigManager.GetAppSetting<int>(Keys.Config.MinimumLoggingLevel);
        protected static string LogPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.LogPath);
        protected static string OutputFolderPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.OutputFolderPath);
        protected static string Level1FolderFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.Level1FolderFormat);

        protected static IProcessor Processor { get; private set; }
        protected static ILogger Logger { get; private set; }
        protected static IDictionary<ExportType, IExporter> Exporters { get; private set; }
        protected static TMService TMService { get; private set; }

        static void Main(string[] args)
        {
            //TODO: create a progress marker to show how much was processed whenever a command is issued.
            System.Console.WriteLine("Transfermarkt Web Scrapper\n");

            Logger = LoggerFactory.GetLogger((LogLevel)MinimumLoggingLevel);
            Exporters = new Dictionary<ExportType, IExporter>
            {
                { ExportType.JSON, new JsonExporter(OutputFolderPath, Level1FolderFormat) }
            };
            TMService = new TMService
            {
                Logger = Logger,
                BaseURL = BaseURL
            };

            TMCommandProcessor.ContinentFileNameFormat = ContinentFileNameFormat;
            TMCommandProcessor.CompetitionFileNameFormat = CompetitionFileNameFormat;
            TMCommandProcessor.ClubFileNameFormat = ClubFileNameFormat;
            Processor = new TMCommandProcessor(Logger, Exporters, TMService);
            Processor.Run();
        }
    }
}
