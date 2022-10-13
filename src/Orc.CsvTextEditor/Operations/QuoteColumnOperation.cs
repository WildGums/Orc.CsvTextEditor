namespace Orc.CsvTextEditor.Operations
{
    using Catel.Logging;

    internal class QuoteColumnOperation : OperationBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public QuoteColumnOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }

        public override void Execute()
        {
            var location = _csvTextEditorInstance.GetLocation();
            var startPosition = location.Column.Offset + location.Line.Offset;
            var endPosition = startPosition + location.Column.Width;

            var text = _csvTextEditorInstance.GetText();

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

            _csvTextEditorInstance.SetText(text);
            _csvTextEditorInstance.GotoPosition(location.Offset + offsetDelta);

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
    }
}
