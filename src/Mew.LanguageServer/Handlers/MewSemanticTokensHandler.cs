using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Mew.LangServer.Handlers;

public sealed class MewSemanticTokensHandler : SemanticTokensHandlerBase
{
    private readonly MewBufferManager _bufferManager;
    private readonly SemanticTokensLegend _legend;

    public MewSemanticTokensHandler(MewBufferManager bufferManager)
    {
        _bufferManager = bufferManager ?? throw new ArgumentNullException(nameof(bufferManager));
        _legend = new SemanticTokensLegend
        {
            TokenModifiers = new Container<SemanticTokenModifier>(SemanticTokenModifier.Defaults),
            TokenTypes = new Container<SemanticTokenType>(SemanticTokenType.Defaults),
        };
    }

    protected override SemanticTokensRegistrationOptions CreateRegistrationOptions(SemanticTokensCapability capability, ClientCapabilities clientCapabilities)
    {
        return new SemanticTokensRegistrationOptions
        {
            DocumentSelector = DocumentSelector.ForLanguage("mew"),
            Legend = _legend,
            Full = new SemanticTokensCapabilityRequestFull
            {
                Delta = true,
            },
            Range = true,
        };
    }

    protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SemanticTokensDocument(_legend));
    }

    protected override Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken)
    {
        // Get statements
        var buffer = _bufferManager.GetBuffer(identifier.TextDocument.Uri);
        if (buffer == null)
        {
            return Task.CompletedTask;
        }

        var context = new List<(IPositionable Token, SemanticTokenType Type)>();
        var semanticModel = buffer.Compilation.GetSemanticModel();
        foreach (var tree in semanticModel.SyntaxTrees)
        {
            tree.Root.Accept(SemanticTokensVisitor.Shared, context);
        }

        foreach (var (token, tokenType) in context.OrderBy(t => t.Token.Span.Position))
        {
            var tokenRange = buffer.GetRange(token.Span);
            foreach (var range in SplitRanges(buffer, tokenRange))
            {
                builder.Push(range, (SemanticTokenType?)tokenType);
            }
        }

        return Task.CompletedTask;
    }

    private static IEnumerable<Range> SplitRanges(MewBuffer buffer, Range range)
    {
        if (range.Start.Line == range.End.Line)
        {
            yield return range;
            yield break;
        }

        var start = range.Start;
        var end = range.End;

        while (start.Line < end.Line)
        {
            var lineEnd = buffer.Lines[start.Line].Length;

            yield return new Range(
                new Position(start.Line, 0),
                new Position(start.Line, lineEnd));

            start = new Position(start.Line + 1, 0);
        }

        yield return new Range(start, end);
    }
}
