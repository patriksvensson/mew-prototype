namespace Mew.CodeAnalysis.Semantics;

public sealed class SymbolCache
{
    private readonly object _lock;
    private readonly SemanticModel _model;
    private Dictionary<Syntax, Symbol>? _lookup;

    public SymbolCache(SemanticModel model)
    {
        _lock = new object();
        _model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public Symbol? GetSymbol(Syntax syntax)
    {
        if (_lookup == null)
        {
            BuildSymbolCache();
        }

        _lookup!.TryGetValue(syntax, out var symbol);
        return symbol;
    }

    private void BuildSymbolCache()
    {
        lock (_lock)
        {
            if (_lookup != null)
            {
                return;
            }

            var context = new Builder.Context();

            // Add function declarations to symbol table
            foreach (var (symbol, block) in _model.Functions)
            {
                context.Symbols[symbol.Declaration.Name] = symbol;

                if (symbol.Declaration.ReturnType != null)
                {
                    context.Symbols[symbol.Declaration.ReturnType] = symbol.ReturnType;
                }

                foreach (var parameter in symbol.Parameters)
                {
                    context.Symbols[parameter.Declaration] = parameter;

                    if (parameter.Declaration is ParameterSyntax parameterSyntax)
                    {
                        context.Symbols[parameterSyntax.Name] = parameter;
                        context.Symbols[parameterSyntax.Type] = parameter.Type;
                    }
                }

                block.Accept(Builder.Shared, context);
            }

            // Iterate statements
            foreach (var statement in _model.Statements)
            {
                statement.Accept(Builder.Shared, context);
            }

            _lookup = context.Symbols;
        }
    }

    private sealed class Builder : BoundNodeVisitor<Builder.Context>
    {
        public static Builder Shared { get; } = new Builder();

        public sealed class Context
        {
            public Dictionary<Syntax, Symbol> Symbols { get; }

            public Context()
            {
                Symbols = new Dictionary<Syntax, Symbol>();
            }
        }

        protected override void VisitSymbol(BoundNode node, Symbol symbol, Context context)
        {
            context.Symbols[node.Syntax] = symbol;
        }

        public override void VisitFunctionCallExpression(BoundFunctionCallExpression node, Context context)
        {
            if (node.Syntax is FunctionCallExpression functionSyntax)
            {
                context.Symbols[functionSyntax.Name] = node.Function;
            }

            base.VisitFunctionCallExpression(node, context);
        }
    }
}
