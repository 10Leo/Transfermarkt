using System;

namespace Transfermarkt.Core.Contracts
{
    public interface IElementParser<TNode, TValue>
    {
        IConverter<TValue> Converter { get; set; }

        event EventHandler<CustomEventArgs> OnSuccess;
        event EventHandler<CustomEventArgs> OnFailure;

        bool CanParse(TNode node);
        TValue Parse(TNode node);
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