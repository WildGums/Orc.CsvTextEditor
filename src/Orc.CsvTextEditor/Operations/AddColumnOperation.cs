// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddColumnOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    using Catel.Logging;

    internal class QuoteColumnOperation : OperationBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Constructors
        public QuoteColumnOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
        #endregion

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
                var quoteStr = Symbols.Quote.ToString();
                text = text.Insert(startPosition, quoteStr)
                    .Insert(endPosition, quoteStr);

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
    }

    internal class AddColumnOperation : OperationBase
    {
        #region Constructors
        public AddColumnOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            if (CsvTextEditorInstance.IsCaretWithinQuotedField())
            {
                CsvTextEditorInstance.InsertAtCaret(Symbols.Comma);
                return;
            }
            
            var location = CsvTextEditorInstance.GetLocation();

            var column = location.Column;
            var offset = location.GetOffsetInLine();

            var newColumnIndex = -1;
            if (column.Width > 1)
            {
                if (offset == column.Offset)
                {
                    newColumnIndex = column.Index;
                }

                if (offset == location.Column.Offset + location.Column.Width - 1)
                {
                    newColumnIndex = column.Index + 1;
                }
            }
            else
            {
                newColumnIndex = column.Index + 1;
            }

            if (newColumnIndex >= 0)
            {
                var oldText = CsvTextEditorInstance.GetText();
                CreateColumn(oldText, newColumnIndex, location);

                return;
            }

            var startPosition = location.Column.Offset + location.Line.Offset;
            var quoteStr = Symbols.Quote.ToString();

            var text = CsvTextEditorInstance.GetText();
            
            text = text.Insert(startPosition, quoteStr);
            text = text.Insert(location.Offset + 1, Symbols.Comma.ToString());
            text = text.Insert(startPosition + location.Column.Width + 1, quoteStr);

            CsvTextEditorInstance.SetText(text);
            CsvTextEditorInstance.GotoPosition(location.Offset + 2);
        }

        private void CreateColumn(string oldText, int newColumnIndex, Location location)
        {
            var newText = oldText.InsertCommaSeparatedColumn(
                newColumnIndex,
                CsvTextEditorInstance.LinesCount,
                CsvTextEditorInstance.ColumnsCount,
                CsvTextEditorInstance.LineEnding);

            CsvTextEditorInstance.SetText(newText);
            CsvTextEditorInstance.GotoPosition(location.Line.Index, newColumnIndex);
        }
        #endregion
    }
}
