namespace Mew.CodeAnalysis;

[DebuggerDisplay("{Code,nq}: {Message,nq} ({Severity,nq})")]
public sealed class DiagnosticDescriptor
{
    public string Code { get; }
    public Severity Severity { get; }
    public string Message { get; }
    public List<Note> Notes { get; }

    public DiagnosticDescriptor(string code, Severity severity, string message)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Severity = severity;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Notes = new List<Note>();
    }

    public DiagnosticDescriptor WithNote(TextSpan span, string message)
    {
        Notes.Add(new Note(span, message));
        return this;
    }

    public sealed class Note
    {
        public TextSpan Span { get; }
        public string Message { get; }

        public Note(TextSpan span, string message)
        {
            Span = span;
            Message = message ?? "?";
        }
    }

    public static DiagnosticDescriptor MEW2004_Unexpected_Character(char c) =>
        new DiagnosticDescriptor("MEW2004", Severity.Error, $"Unexpected character '{c}'");

    public static DiagnosticDescriptor MEW1000_Expected_Semicolon { get; } =
        new DiagnosticDescriptor("MEW1000", Severity.Error, "Expected ';'");

    public static DiagnosticDescriptor MEW1001_Expected_Variable_Name { get; } =
        new DiagnosticDescriptor("MEW1001", Severity.Error, "Expected variable name");

    public static DiagnosticDescriptor MEW1003_Expected_RParen { get; } =
        new DiagnosticDescriptor("MEW1003", Severity.Error, "Expected ')'");

    public static DiagnosticDescriptor MEW1005_Unterminated_String_Literal { get; } =
        new DiagnosticDescriptor("MEW1005", Severity.Error, "Unterminated string literal");

    public static DiagnosticDescriptor MEW1006_Expression_Statements_Not_Allowed { get; } =
        new DiagnosticDescriptor("MEW1006", Severity.Error, "An expression can not be used as a statement");

    public static DiagnosticDescriptor MEW1007_Invalid_Assignment_Target { get; } =
        new DiagnosticDescriptor("MEW1007", Severity.Error, "Invalid assignment target");

    public static DiagnosticDescriptor MEW1008_Expected_RBrace_After_Block { get; } =
        new DiagnosticDescriptor("MEW1008", Severity.Error, "Expected '}' after block");

    public static DiagnosticDescriptor MEW1009_Expected_Function_Name { get; } =
        new DiagnosticDescriptor("MEW1009", Severity.Error, "Expected function name");

    public static DiagnosticDescriptor MEW1010_Expected_LParam_After_Function_Name { get; } =
        new DiagnosticDescriptor("MEW1010", Severity.Error, "Expected '(' after function name");

    public static DiagnosticDescriptor MEW1011_Parameter_Limit_Reached { get; } =
        new DiagnosticDescriptor("MEW1011", Severity.Error, "Can't have more than 255 parameters");

    public static DiagnosticDescriptor MEW1012_Expected_Parameter_Name { get; } =
        new DiagnosticDescriptor("MEW1012", Severity.Error, "Expected parameter name");

    public static DiagnosticDescriptor MEW1013_Expected_RParam_After_Parameters { get; } =
        new DiagnosticDescriptor("MEW1013", Severity.Error, "Expected ')' after parameters");

    public static DiagnosticDescriptor MEW1014_Expected_LBrace_Before_Function_Body { get; } =
        new DiagnosticDescriptor("MEW1014", Severity.Error, "Expected '{' before function body");

    public static DiagnosticDescriptor MEW1015_Expected_LParen_After_If { get; } =
        new DiagnosticDescriptor("MEW1015", Severity.Error, "Expected '(' after 'if'");

    public static DiagnosticDescriptor MEW1016_Expected_RParen_After_Condition { get; } =
        new DiagnosticDescriptor("MEW1016", Severity.Error, "Expected ')' after condition");

    public static DiagnosticDescriptor MEW1017_Expected_Colon { get; } =
        new DiagnosticDescriptor("MEW1017", Severity.Error, "Expected ':'");

    public static DiagnosticDescriptor MEW1018_Expected_Equal { get; } =
        new DiagnosticDescriptor("MEW1018", Severity.Error, "Expected '='");

    public static DiagnosticDescriptor MEW1019_Unrecognized_Expression { get; } =
        new DiagnosticDescriptor("MEW1019", Severity.Error, "Expected a literal value, a parenthesized expression, or a function call at this location");

    public static DiagnosticDescriptor MEW1020_Expected_LBrace_Before_Block { get; } =
        new DiagnosticDescriptor("MEW1020", Severity.Error, "Expected '{'");

    public static DiagnosticDescriptor MEW1021_Expected_Parameter_Name { get; } =
        new DiagnosticDescriptor("MEW1021", Severity.Error, "Expected parameter name");

    public static DiagnosticDescriptor MEW1022_Expected_Type { get; } =
        new DiagnosticDescriptor("MEW1022", Severity.Error, "Expected type");

    public static DiagnosticDescriptor MEW1023_Expected_Function_Name { get; } =
        new DiagnosticDescriptor("MEW1023", Severity.Error, "Expected function name");

    public static DiagnosticDescriptor MEW1024_Expected_Struct_Name { get; } =
        new DiagnosticDescriptor("MEW1024", Severity.Error, "Expected struct name");

    public static DiagnosticDescriptor MEW1025_Expected_LBrace_Before_Struct_Body { get; } =
        new DiagnosticDescriptor("MEW1025", Severity.Error, "Expected '{' before struct body");

    public static DiagnosticDescriptor MEW1026_Expected_RBrace_After_Struct_Body { get; } =
        new DiagnosticDescriptor("MEW1026", Severity.Error, "Expected '}' after struct body");

    public static DiagnosticDescriptor MEW1027_Expected_Field_Name { get; } =
        new DiagnosticDescriptor("MEW1027", Severity.Error, "Expected field name");

    public static DiagnosticDescriptor MEW1028_Expected_Fn_Keyword { get; } =
        new DiagnosticDescriptor("MEW1028", Severity.Error, "Expected 'fn' keyword");

    public static DiagnosticDescriptor MEW1029_Expected_Identifier_Or_Function_Call { get; } =
        new DiagnosticDescriptor("MEW1029", Severity.Error, "Expected an identifier or a function call at this location");

    public static DiagnosticDescriptor MEW1030_Expected_Identifier_Found_Keyword(string lexeme) =>
        new DiagnosticDescriptor("MEW1030", Severity.Error, $"Expected an identifier but encountered reserved keyword '{lexeme}'");

    public static DiagnosticDescriptor MEW1031_Expected_LBrace { get; } =
        new DiagnosticDescriptor("MEW1031", Severity.Error, "Expected '{'");

    public static DiagnosticDescriptor MEW1032_Expected_RBrace { get; } =
        new DiagnosticDescriptor("MEW1032", Severity.Error, "Expected '}'");

    public static DiagnosticDescriptor MEW1033_Expected_Type_Name { get; } =
        new DiagnosticDescriptor("MEW1033", Severity.Error, "Expected type name");

    public static DiagnosticDescriptor MEW1034_Expected_LBrace_Before_Type_Body { get; } =
        new DiagnosticDescriptor("MEW1034", Severity.Error, "Expected '{' before type body");

    public static DiagnosticDescriptor MEW1035_Expected_LBrace_After_Type_Body { get; } =
        new DiagnosticDescriptor("MEW1035", Severity.Error, "Expected '}' after type body");

    public static DiagnosticDescriptor MEW1036_Expected_Function_Keyword { get; } =
        new DiagnosticDescriptor("MEW1036", Severity.Error, "Expected 'fn' keyword");

    public static DiagnosticDescriptor MEW1100_Undeclared_Variable(string lexeme) =>
        new DiagnosticDescriptor("MEW1100", Severity.Error, $"Undeclared variable '{lexeme}'");

    public static DiagnosticDescriptor MEW1101_Not_A_Variable(string lexeme) =>
        new DiagnosticDescriptor("MEW1101", Severity.Error, $"'{lexeme}' is not a variable");

    public static DiagnosticDescriptor MEW1102_Unknown_Type(string lexeme) =>
        new DiagnosticDescriptor("MEW1102", Severity.Error, $"Undeclared type '{lexeme}'");

    public static DiagnosticDescriptor MEW1103_Variable_Already_Declared(string lexeme) =>
        new DiagnosticDescriptor("MEW1103", Severity.Error, $"Variable '{lexeme}' has already been declared");

    public static DiagnosticDescriptor MEW1104_Function_Already_Declared(string lexeme) =>
        new DiagnosticDescriptor("MEW1104", Severity.Error, $"Function '{lexeme}' has already been declared");

    public static DiagnosticDescriptor MEW1105_Binary_Operator_Not_Defined(string op, BoundExpression left, BoundExpression right) =>
        new DiagnosticDescriptor("MEW1105", Severity.Error, $"Binary operator '{op}' is not defined for types '{left.Type.Name}' and '{right.Type.Name}'")
            .WithNote(left.Syntax.Span, $"This is of type '{left.Type.Name}'")
            .WithNote(right.Syntax.Span, $"This is of type '{right.Type.Name}'");

    public static DiagnosticDescriptor MEW1106_Logical_Operator_Not_Defined(string op, BoundExpression left, BoundExpression right) =>
        new DiagnosticDescriptor("MEW1105", Severity.Error, $"Logical operator '{op}' is not defined for types '{left.Type.Name}' and '{right.Type.Name}'")
            .WithNote(left.Syntax.Span, $"This is of type '{left.Type.Name}'")
            .WithNote(right.Syntax.Span, $"This is of type '{right.Type.Name}'");

    public static DiagnosticDescriptor MEW1107_Undefined_Function(string lexeme) =>
        new DiagnosticDescriptor("MEW1107", Severity.Error, $"Undefined function '{lexeme}'");

    public static DiagnosticDescriptor MEW1108_Not_A_Function(string lexeme) =>
        new DiagnosticDescriptor("MEW1108", Severity.Error, $"'{lexeme}' is not a function");

    public static DiagnosticDescriptor MEW1109_Invalid_Parameter_Count(string lexeme, int expected, int actual) =>
        new DiagnosticDescriptor("MEW1109", Severity.Error, $"Function '{lexeme}' requires {expected} arguments but was given {actual}");

    public static DiagnosticDescriptor MEW1110_Cannot_Convert_Implicitly(TypeSymbol from, TypeSymbol to) =>
        new DiagnosticDescriptor("MEW1110", Severity.Error, $"Cannot convert type '{from.Name}' to '{to.Name}'");

    public static DiagnosticDescriptor MEW1111_Unreachable_Code { get; } =
        new DiagnosticDescriptor("MEW1111", Severity.Error, "Unreachable code detected");

    public static DiagnosticDescriptor MEW1112_Break_Used_Outside_Of_Loop { get; } =
        new DiagnosticDescriptor("MEW1112", Severity.Error, "No enclosing loop out of which to break");

    public static DiagnosticDescriptor MEW1113_Continue_Used_Outside_Of_Loop { get; } =
        new DiagnosticDescriptor("MEW1113", Severity.Error, "No enclosing loop out of which to continue");

    public static DiagnosticDescriptor MEW1114_Unary_Operator_Not_Defined(string op, TypeSymbol type) =>
        new DiagnosticDescriptor("MEW1114", Severity.Error, $"Unary operator '{op}' is not defined for type '{type.Name}'");

    public static DiagnosticDescriptor MEW1115_Enclosing_Function_Does_Not_Return_A_Value { get; } =
        new DiagnosticDescriptor("MEW1115", Severity.Error, "Enclosing function does not return a value");

    public static DiagnosticDescriptor MEW1116_Missing_Return_Expression(TypeSymbol type) =>
        new DiagnosticDescriptor("MEW1116", Severity.Error, $"An expression of type '{type.Name}' was expected");

    public static DiagnosticDescriptor MEW1117_Wrong_Return_Type(TypeSymbol expected, TypeSymbol actual) =>
        new DiagnosticDescriptor("MEW1117", Severity.Error, $"Wrong return type. Expected '{expected.Name}' but was given '{actual.Name}'");

    public static DiagnosticDescriptor MEW1118_No_Overload_With_Argument_Count(string lexeme, int actual) =>
        new DiagnosticDescriptor("MEW1118", Severity.Error, $"No overload for function '{lexeme}' takes {actual} arguments");

    public static DiagnosticDescriptor MEW1119_Type_Already_Declared(string lexeme) =>
        new DiagnosticDescriptor("MEW1119", Severity.Error, $"Type '{lexeme}' has already been declared");

    public static DiagnosticDescriptor MEW9999_Not_Implemented { get; } =
        new DiagnosticDescriptor("MEW9999", Severity.Error, "Not implemented");

    public Diagnostic ToDiagnostic(Location location)
    {
        var diagnostic = new Diagnostic(Code, location, Severity, Message);

        if (Notes.Count > 0)
        {
            foreach (var note in Notes)
            {
                diagnostic.Notes.Add(
                    new DiagnosticNote(
                        new Location(location.Path, note.Span),
                        note.Message));
            }
        }

        return diagnostic;
    }
}
