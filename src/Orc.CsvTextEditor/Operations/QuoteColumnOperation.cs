namespace Orc.CsvTextEditor.Operations
{
    using Catel.Logging;

    internal class QuoteColumnOperation : OperationBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        public QuoteColumnOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            var location = CsvTextEditorInstance.GetLocation();
            var startPosition = location.Column.Offset + location.Line.Offset;
            var endPosition = startPosition + location.Column.Width;

            var text = CsvTextEditorInstance.GetText();

            var quotesRemoved = false;
            if (TryRemoveQuoteFromPosition(endPosition - 2, text, out var outputText))
            {
                text = outputText;
                quotesRemoved = true;
            }

            if (TryRemoveQuoteFromPosition(startPosition, text, out outputText))
            {
                text = outputText;
                quotesRemoved = true;
            }

            var offsetDelta = -1;
            if (!quotesRemoved)
            {
                text = text.Insert(startPosition, SymbolsStr.Quote)
                    .Insert(endPosition, SymbolsStr.Quote);

                offsetDelta = 1;
            }

            CsvTextEditorInstance.SetText(text);
            CsvTextEditorInstance.GotoPosition(location.Offset + offsetDelta);

            Log.Debug($"{nameof(QuoteColumnOperation)} executed; quotes were {(quotesRemoved ? "removed" : "added")}");
        }

        private static bool TryRemoveQuoteFromPosition(int symbolPosition, string inputText, out string outputText)
        {
            outputText = inputText;
            if (symbolPosition >= inputText.Length || symbolPosition < 0)
            {
                return false;
            }

            var startSymbol = inputText[symbolPosition];
            if (!Equals(startSymbol, Symbols.Quote))
            {
                return false;
            }

            outputText = inputText.Remove(symbolPosition, 1);

            return true;
        }
        #endregion
    }
}
