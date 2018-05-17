// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddColumnOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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

            var oldText = _csvTextEditorInstance.GetText();

            var location = _csvTextEditorInstance.GetLocation();

            var newColumnIndex = location.GetOffsetInLine() == location.Column.Offset 
                ? location.Column.Index
                : location.Column.Index + 1;

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