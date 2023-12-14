namespace Orc.CsvTextEditor
{
    public static class Symbols
    {
        public const char Comma = ',';
        public const char Quote = '"';
        public const char NewLineStart = '\r';
        public const char NewLineEnd = '\n';
        public const char HorizontalTab = '\t';
        public const char Space = ' ';
        public const char VerticalBar = '|';
    }

    public static class SymbolsStr
    {
        public static readonly string Comma = Symbols.Comma.ToString();
        public static readonly string Quote = Symbols.Quote.ToString();
    }
}
