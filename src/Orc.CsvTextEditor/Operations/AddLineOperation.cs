// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddLineOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
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
            var location = CsvTextEditorInstance.GetLocation();

            var oldText = CsvTextEditorInstance.GetText();
            var text = oldText.InsertLineWithTextTransfer(location.Line.Index + 1, location.GetOffsetInLine(), CsvTextEditorInstance.ColumnsCount, CsvTextEditorInstance.LineEnding);

            CsvTextEditorInstance.SetText(text);
            CsvTextEditorInstance.GotoPosition(location.Line.Index + 1, 0);
        }
        #endregion
    }
}
