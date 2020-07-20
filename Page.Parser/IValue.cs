using System;

namespace Page.Parser.Contracts
{
    public interface IValue
    {
        Type Type { get; }
        string ToString();
    }
}
