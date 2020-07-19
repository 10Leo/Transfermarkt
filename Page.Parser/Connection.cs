using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page.Parser.Contracts
{
    public abstract class Connection<TNode> : IConnection<TNode>
    {
        public Func<TNode> GetNodeFunc { get; set; }

        public abstract bool IsConnected { get; }

        public abstract TNode Connect(string url);

        public TNode GetNode()
        {
            if (GetNodeFunc != null)
            {
                return GetNodeFunc();
            }

            return default;
        }

        public abstract void Reset();
    }
}
