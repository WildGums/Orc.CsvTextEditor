// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    public static class Symbols
    {
        #region Constants
        public const char Comma = ',';
        public const char Quote = '"';
        public const char NewLineStart = '\r';
        public const char NewLineEnd = '\n';
        public const char HorizontalTab = '\t';
        public const char Space = ' ';
        public const char VerticalBar = '|';
        #endregion
    }

    public static class SymbolsStr
    {
        public static readonly string Comma = Symbols.Comma.ToString();
        public static readonly string Quote = Symbols.Quote.ToString();
    }
}
