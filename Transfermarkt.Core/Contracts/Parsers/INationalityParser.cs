using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core.Contracts.Parsers
{
    public interface INationalityParser<TNode> : IElementParser<TNode, Nationality?>
    {
    }
}