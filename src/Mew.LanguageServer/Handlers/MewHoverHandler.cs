using Mew.CodeAnalysis.Semantics;

namespace Mew.LangServer.Handlers;

public sealed class MewHoverHandler : HoverHandlerBase
{
    private readonly MewBufferManager _bufferManager;
    private readonly MewLogger _logger;

    public MewHoverHandler(
        MewBufferManager bufferManager,
        MewLogger logger)
    {
        _bufferManager = bufferManager ?? throw new ArgumentNullException(nameof(bufferManager));
        _logger = logger?.ForOrigin("MewHoverHandler") ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override HoverRegistrationOptions CreateRegistrationOptions(HoverCapability capability, ClientCapabilities clientCapabilities)
    {
        return new HoverRegistrationOptions
        {
            DocumentSelector = DocumentSelector.ForLanguage("mew"),
        };
    }

    public override Task<Hover?> Handle(HoverParams request, CancellationToken cancellationToken)
    {
        var buffer = _bufferManager.GetBuffer(request.TextDocument.Uri);
        if (buffer == null)
        {
            _logger.LogError("Hover: Could not find buffer");
            return Task.FromResult(default(Hover));
        }

        var offset = buffer.GetOffsetFromPosition(request.Position);
        if (offset == null)
        {
            _logger.LogError("Hover: Could not find position in buffer");
            return Task.FromResult(default(Hover));
        }

        var syntax = SyntaxFinder.Find(buffer, offset.Value);
        if (syntax == null)
        {
            _logger.LogError("Hover: Could not find syntax node in buffer");
            return Task.FromResult(default(Hover));
        }

        var semanticModel = buffer.Compilation.GetSemanticModel();
        var symbol = semanticModel.SymbolCache.GetSymbol(syntax);
        if (symbol == null)
        {
            return Task.FromResult(default(Hover));
        }

        var markdown = GetMarkdown(symbol);
        if (markdown == null)
        {
            return Task.FromResult(default(Hover));
        }

        return Task.FromResult<Hover?>(new Hover
        {
            Range = buffer.GetRange(syntax.Span),
            Contents = new MarkedStringsOrMarkupContent(new MarkupContent
            {
                Kind = MarkupKind.Markdown,
                Value = markdown,
            }),
        });
    }

    private static string? GetMarkdown(Symbol symbol)
    {
        switch (symbol)
        {
            case ParameterSymbol parameter:
                return GetCodeBlock($"{parameter.Name} : {parameter.Type.Name}", "Parameter");
            case VariableSymbol variable:
                return GetCodeBlock($"{variable.Name} : {variable.Type.Name}", "Variable");
            case UndeclaredTypeSymbol undeclared:
                return GetCodeBlock(undeclared.Name, "Undeclared type");
            case PrimitiveTypeSymbol type:
                return GetCodeBlock(type.Name, "Primitive type");
            case TypeSymbol type:
                return GetCodeBlock(type.Name, "Type");
            case ExternalFunctionSymbol function:
                return GetCodeBlock(
                    $"{function.Name}() -> {function.ReturnType.Name}",
                    $"External function  \nImported from **{function.Library}**");
            case DeclaredFunctionSymbol function:
                return GetCodeBlock($"{function.Name}() -> {function.ReturnType.Name}", "Function");
            default:
                return null;
        }
    }

    private static string GetCodeBlock(string content, string? description)
    {
        if (description == null)
        {
            return $"```\n{content}\n```\n";
        }

        return $"```\n{content}\n```\n  {description}";
    }

    private sealed class SyntaxFinder : SyntaxVisitor<SyntaxFinder.Context>
    {
        public static SyntaxFinder Shared { get; } = new SyntaxFinder();

        public sealed class Context
        {
            public int Offset { get; }
            public Syntax? Result { get; set; }

            public Context(int offset)
            {
                Offset = offset;
            }
        }

        public static Syntax? Find(MewBuffer buffer, int offset)
        {
            var context = new Context(offset);

            var semanticModel = buffer.Compilation.GetSemanticModel();
            foreach (var tree in semanticModel.SyntaxTrees)
            {
                foreach (var stmt in tree.Root.Statements)
                {
                    stmt.Accept(Shared, context);
                    if (context.Result != null)
                    {
                        return context.Result;
                    }
                }
            }

            return null;
        }

        protected override void Visit(Syntax? syntax, Context context)
        {
            if (syntax == null)
            {
                return;
            }

            if (syntax.Span.Contains(context.Offset))
            {
                if (syntax is not SyntaxToken)
                {
                    context.Result = syntax;
                }

                // See if there is any better candidate
                base.Visit(syntax, context);
            }
        }
    }
}
