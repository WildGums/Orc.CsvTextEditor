// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddColumnOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    internal class QuoteColumnOperation : OperationBase
    {
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
            text = TryRemoveQuoteFromPosition(endPosition - 2, text, out var quotesRemoved);
            text = TryRemoveQuoteFromPosition(startPosition, text, out quotesRemoved);

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
        }

        private static string TryRemoveQuoteFromPosition(int symbolPosition, string text, out bool quotesRemoved)
        {
            quotesRemoved = false;
            if (symbolPosition >= text.Length || symbolPosition < 0)
            {
                return text;
            }

            var startSymbol = text[symbolPosition];
            if (!Equals(startSymbol, Symbols.Quote))
            {
                return text;
            }

            text = text.Remove(symbolPosition, 1);
            quotesRemoved = true;

            return text;
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
