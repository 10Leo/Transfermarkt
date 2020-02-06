﻿using System;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IElementParser<TNode, TElement, TValue> where TElement : IElement
    {
        TElement Element { get; }
        IConverter<TValue> Converter { get; set; }

        event EventHandler<CustomEventArgs> OnSuccess;
        event EventHandler<CustomEventArgs> OnFailure;

        bool CanParse(TNode node);
        TElement Parse(TNode node);
    }

    public class CustomEventArgs : EventArgs
    {
        public string Message { get; }

        public CustomEventArgs(string message)
        {
            Message = message;
        }
    }
}