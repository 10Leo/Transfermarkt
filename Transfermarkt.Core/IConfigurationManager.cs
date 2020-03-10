using System;

namespace Transfermarkt.Core.Contracts
{
    public interface IConfigurationManager
    {
        T GetAppSetting<T>(string key) where T : IConvertible;
    }
}
