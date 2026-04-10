namespace CobolScanner;

public enum TokenType
{
    // Structural
    Keyword,
    Identifier,

    // Literals
    IntegerLiteral,
    DecimalLiteral,
    StringLiteral,
    FigurativeConstant,   // SPACE, SPACES, ZERO, ZEROS, ZEROES, HIGH-VALUE, LOW-VALUE, QUOTE, ALL

    // Symbols
    Period,               // .
    Comma,                // ,
    Semicolon,            // ;
    LeftParen,            // (
    RightParen,           // )
    Colon,                // :
    Hyphen,               // - (arithmetic or word separator context)
    Plus,                 // +
    Asterisk,             // *
    Slash,                // /
    Equal,                // =
    GreaterThan,          // >
    LessThan,             // <
    GreaterOrEqual,       // >=
    LessOrEqual,          // <=
    NotEqual,             // <>

    // PIC string (e.g., X(8), S9(7)V99)
    PictureString,

    // Special lines
    CommentLine,          // lines with * or / in column 7
    DebugLine,            // lines with D in column 7
    ContinuationLine,     // lines with - in column 7
    SequenceNumber,       // columns 1-6

    // End of file
    EOF,

    // Unknown / error token
    Unknown
}
