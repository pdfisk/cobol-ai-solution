namespace CobolScanner;

public class Token
{
    TokenType type;
    string value;
    int row;
    int col;
    string fileName;

    public Token(TokenType typeArg, int rowArg, int colArg, string valueArg = "")
    {
        type = typeArg;
        row = rowArg;
        col = colArg;
        value = valueArg;
        fileName = "";
    }

    public override string ToString() =>
        $"[{type,-22}] {value,-40} ({fileName}:{row}:{col})";
}
