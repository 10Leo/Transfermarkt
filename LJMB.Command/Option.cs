using System;
using System.Collections.Generic;

namespace LJMB.Command
{
    public abstract class Option : IOption
    {
        public struct Exceptions
        {
            public const string OPTION_NOT_FOUND_ERROR_MSG = "{0}' option required.";
            public const string ARGUMENTS_NOT_FOUND_ERROR_MSG = "{0}'s arguments not specified.";
            public const string TOO_MUCH_ARGUMENTS_ERROR_MSG = "{0}' too much arguments specified.";
        }

        public string Name { get; protected set; }
        public bool Active { get; set; } = false;
        public bool Required { get; set; } = false;
        public string Usage { get; set; }

        public ISet<string> AllowedAlias { get; protected set; }

        public ISet<IArgument> Args { get; protected set; } = new HashSet<IArgument>();

        public virtual void Validate()
        {
            if (Required)
            {
                if (!Active)
                {
                    throw new Exception(string.Format(Exceptions.OPTION_NOT_FOUND_ERROR_MSG, this.Name));
                }
            }
        }

        public abstract void Parse(string toParse);

        public virtual void Reset()
        {
            Active = false;
            Args.Clear();
        }
    }
}
