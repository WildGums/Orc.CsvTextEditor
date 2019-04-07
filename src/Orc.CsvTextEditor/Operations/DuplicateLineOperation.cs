// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DuplicateLineOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    public class DuplicateLineOperation : OperationBase
    {
        #region Constructors
        public DuplicateLineOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            var location = CsvTextEditorInstance.GetLocation();

            var text = CsvTextEditorInstance.GetText();
            text = text.DuplicateTextInLine(location.Line.Offset, location.Line.Offset + location.Line.Length, CsvTextEditorInstance.LineEnding);

            CsvTextEditorInstance.SetText(text);
            CsvTextEditorInstance.GotoPosition(location.Line.Index + 1, location.Column.Index);
        }
        #endregion
    }
}
