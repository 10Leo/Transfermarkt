using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core
{
    public class ConfigManager : IConfigurationManager
    {
        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings["SettingsFolderPath"].ToString();
        }
    }
}
