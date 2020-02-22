﻿namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Element : IElement
    {
        public string InternalName { get; }
        public string Name { get; }
        public IValue Value { get; set; }

        public Element(string internalName, string name)
        {
            InternalName = internalName;
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("[{0}: {1}]", InternalName, Value);
        }
    }
}
