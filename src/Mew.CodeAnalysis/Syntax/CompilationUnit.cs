namespace Mew.CodeAnalysis;

public sealed class CompilationUnit : Syntax
{
    public ImmutableArray<StatementSyntax> Statements { get; }

    public override TextSpan Span => TextSpan.Between(Statements);
    public override bool IsValid => Statements.All(n => n.IsValid);

    public CompilationUnit(SyntaxTree syntaxTree, IEnumerable<StatementSyntax> nodes)
        : base(syntaxTree)
    {
        if (nodes is null)
        {
            throw new ArgumentNullException(nameof(nodes));
        }

        Statements = ImmutableArray.CreateRange(nodes);
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitProgram(this, context);
    }
}