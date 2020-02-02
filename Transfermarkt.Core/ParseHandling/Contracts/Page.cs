﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Page<TNode> : IPage<IDomain, TNode, IElement>
    {
        public IDomain Domain { get; set; }

        public IConnection<TNode> Connection { get; set; }

        public IReadOnlyList<ISection<IDomain, TNode, IElement>> Sections { get; set; }

        public Page(IConnection<TNode> connection)
        {
            this.Connection = connection ?? throw new Exception("Can't pass a null connection.");
        }

        #region Contract

        public virtual IDomain Parse(string url)
        {
            this.Connection.Connect(url);

            if (Sections != null)
            {
                foreach (var section in Sections)
                {
                    section.Parse(this);
                }
            }

            return this.Domain;
        }

        public virtual void Save()
        {
        }

        #endregion Contract
    }
}
