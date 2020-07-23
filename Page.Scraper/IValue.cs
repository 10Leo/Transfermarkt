using System;

namespace Page.Scraper.Contracts
{
    public interface IValue
    {
        Type Type { get; }
        string ToString();
    }
}
