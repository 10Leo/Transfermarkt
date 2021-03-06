﻿using System;
using System.Collections.Generic;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Page<TValue, TNode> : IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> where TValue : IValue
    {
        public IDomain<TValue> Domain { get; set; }

        public IConnection<TNode> Connection { get; set; }

        public IReadOnlyList<ISection<IElement<TValue>, TValue, TNode>> Sections { get; set; }
        
        public event EventHandler<PageEventArgs> OnAfterParse;
        public event EventHandler<PageEventArgs> OnBeforeParse;

        public Page(IConnection<TNode> connection)
        {
            this.Connection = connection ?? throw new Exception("Can't use a null connection.");
        }

        #region Contract

        public virtual IDomain<TValue> Parse(string url)
        {
            this.Connection.Connect(url);

            OnBeforeParse?.Invoke(this, new PageEventArgs(url));

            if (Sections != null)
            {
                foreach (var section in Sections)
                {
                    section.Parse(this);
                }
            }

            OnAfterParse?.Invoke(this, new PageEventArgs(url));

            return this.Domain;
        }

        public virtual void Save()
        {
        }

        #endregion Contract
    }
}
