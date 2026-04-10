using System.ComponentModel;
using System.Data;

namespace CobolScanner;

public class Lexer
{
    string buffer;
    int pos;
    int row;
    int col;
    List<string> keywords;

    public Lexer(string code)
    {
        buffer = code;
        pos = 0;
        row = 1;
        col = 1;
        keywords = Keywords.AllKeywords();
    }

    public List<Token> tokenize()
    {
        var tokens = new List<Token>();
        while (!isEof())
        {
            var token = nextToken();
            if (token != null)
            {
                tokens.Add(token);
            }
        }
        return tokens;
    }

    void advance()
    {
        if (buffer[pos] == '\n')
        {
            row++;
            col = 1;
        }
        else
        {
            col++;
        }
        pos++;
    }

    int peek()
    {
        if (isEof())
            return -1;
        return buffer[pos];
    }

    Token createToken(TokenType type, string value = "")
    {
        return new Token(type, row, col, value);
    }

    public Token nextToken()
    {
        skipWhitespace();
        if (isEof())
            return createToken(TokenType.EOF);
        if (isLetter(peek()))
        {
            return readIdentifierOrKeyword();
        }
        else if (isDigit(peek()))
        {
            return readNumber();
        }
        setEof();
        return createToken(TokenType.Unknown);
    }

    bool isDigit(int c)
    {
        return c >= '0' && c <= '9';
    }

    void setEof()
    {
        pos = buffer.Length;
    }

    bool isLetter(int c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    bool isWhitespace(int c)
    {
        return c == ' ' || c == '\t' || c == '\n' || c == '\r';
    }

    bool isPeriod(int c)
    {
        return c == '.';
    }

    Token readIdentifierOrKeyword()
    {
        var startCol = col;
        var value = "";
        while (isLetter(peek()) || isDigit(peek()) || peek() == '-')
        {
            value += (char)peek();
            advance();
        }
        // Check if the value is a keyword
        if (Enum.TryParse<TokenType>(value, true, out var type) && type != TokenType.Unknown)
        {
            return createToken(type);
        }
        else
            return createToken(TokenType.Identifier, value);
    }

    Token readNumber()
    {
        var startCol = col;
        var value = "";
        while (isDigit(peek()) || isPeriod(peek()))
        {
            value += (char)peek();
            advance();
        }
        if (value.Count(c => c == '.') > 1)
        {
            return createToken(TokenType.Unknown, value);
        }
        else if (value.Contains('.'))
        {
            return createToken(TokenType.DecimalLiteral, value);
        }
        else
            return createToken(TokenType.IntegerLiteral, value);
    }

    Token readIdentifier()
    {
        var startCol = col;
        var value = "";
        while (isLetter(peek()) || isDigit(peek()) || peek() == '-')
        {
            value += (char)peek();
            advance();
        }
        if (keywords.Contains(value.ToUpper()))
        {
            return createToken(TokenType.Keyword, value);
        }
        else
            return createToken(TokenType.Identifier, value);
    }

    void skipWhitespace()
    {
        while (isWhitespace(peek()))
        {
            advance();
        }
    }

    bool isEof()
    {
        return (pos >= buffer.Length);

    }
}