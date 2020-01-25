using System.Configuration;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core
{
    public class ConfigManager : IConfigurationManager
    {
        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key]?.ToString();
        }
    }
}
