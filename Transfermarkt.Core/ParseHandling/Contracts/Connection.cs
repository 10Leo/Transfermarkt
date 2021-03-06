﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Connection<TNode> : IConnection<TNode>
    {
        public Func<TNode> GetNodeFunc { get; set; }

        public abstract TNode Connect(string url);

        public TNode GetNode()
        {
            if (GetNodeFunc != null)
            {
                return GetNodeFunc();
            }

            return default;
        }
    }
}
