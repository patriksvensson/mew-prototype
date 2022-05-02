namespace Mew.CodeAnalysis;

public enum SyntaxTokenKind
{
    NewLine,
    Unrecognized,

    LParen,
    RParen,
    LBrace,
    RBrace,
    Comma,
    Dot,
    Minus,
    Plus,
    Colon,
    Semicolon,
    Slash,
    Star,
    Percent,
    Bang,
    BangEqual,
    Equal,
    EqualEqual,
    Greater,
    GreaterEqual,
    Less,
    LessEqual,

    // Logical
    Or,
    And,

    // Flow
    Arrow,

    // Literals
    Identifier,
    String,
    Integer,
    Double,
    True,
    False,

    // Keywords
    IntType,
    DoubleType,
    StringType,
    BoolType,
    Fn,
    Return,
    Let,
    Loop,
    If,
    Else,
    Break,
    Continue,
    While,
    Extern,

    // Misc
    Eof,
}
