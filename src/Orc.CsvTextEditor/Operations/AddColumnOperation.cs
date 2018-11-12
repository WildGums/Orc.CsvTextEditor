// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddColumnOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
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
            if (CsvTextEditorInstance.IsCaretWithinQuotedField())
            {
                CsvTextEditorInstance.InsertAtCaret(Symbols.Comma);
                return;
            }

            var oldText = CsvTextEditorInstance.GetText();

            var location = CsvTextEditorInstance.GetLocation();

            var newColumnIndex = location.GetOffsetInLine() == location.Column.Offset
                ? location.Column.Index
                : location.Column.Index + 1;

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
