using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core.Contracts
{
    public interface INationalityParser<TNode> : IElementParser<TNode, Nationality?>
    {
    }
}