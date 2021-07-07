namespace Orc.CsvTextEditor.Operations
{
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
            if (_csvTextEditorInstance.IsCaretWithinQuotedField())
            {
                _csvTextEditorInstance.InsertAtCaret(Symbols.Comma);
                return;
            }

            var location = _csvTextEditorInstance.GetLocation();

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
                var oldText = _csvTextEditorInstance.GetText();
                CreateColumn(oldText, newColumnIndex, location);

                return;
            }

            var startPosition = location.Column.Offset + location.Line.Offset;

            var text = _csvTextEditorInstance.GetText();

            text = text.Insert(startPosition, SymbolsStr.Quote);
            text = text.Insert(location.Offset + 1, SymbolsStr.Comma);
            text = text.Insert(startPosition + location.Column.Width + 1, SymbolsStr.Quote);

            _csvTextEditorInstance.SetText(text);
            _csvTextEditorInstance.GotoPosition(location.Offset + 2);
        }

        private void CreateColumn(string oldText, int newColumnIndex, Location location)
        {
            var newText = oldText.InsertCommaSeparatedColumn(
                newColumnIndex,
                _csvTextEditorInstance.LinesCount,
                _csvTextEditorInstance.ColumnsCount,
                _csvTextEditorInstance.LineEnding);

            _csvTextEditorInstance.SetText(newText);
            _csvTextEditorInstance.GotoPosition(location.Line.Index, newColumnIndex);
        }
        #endregion
    }
}
