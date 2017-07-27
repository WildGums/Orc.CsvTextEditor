// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddLineOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    public class AddLineOperation : OperationBase
    {
        #region Constructors
        public AddLineOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            var location = _csvTextEditorInstance.GetLocation();

            var oldText = _csvTextEditorInstance.GetText();
            var text = oldText.InsertLineWithTextTransfer(location.Line.Index + 1, location.GetOffsetInLine(), _csvTextEditorInstance.ColumnsCount, _csvTextEditorInstance.LineEnding);

            _csvTextEditorInstance.SetText(text);
            _csvTextEditorInstance.GotoPosition(location.Line.Index + 1, location.Column.Index);
        }
        #endregion
    }
}