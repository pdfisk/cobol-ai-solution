namespace CobolScanner;

class CobolScanner
{
    public static void Main(String[] args)
    {
        var fileName = args.Length > 0 ? args[0] : "sample.cobol";

        var dataFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\..\data"));

        if (!Directory.Exists(dataFolder))
        {
            Console.WriteLine($"Data folder not found: {dataFolder}");
            return;
        }

        var files = Directory.GetFiles(dataFolder, "*.cobol", SearchOption.TopDirectoryOnly);

        if (files.Length == 0)
        {
            Console.WriteLine("No .cobol files found.");
            return;
        }

        if (files.Contains(fileName))
        {
            var code = File.ReadAllText(fileName);
            Console.WriteLine(new Lexer(code).tokenize());
            return;
        }

        Console.WriteLine($"File {fileName} not found");
    }
}