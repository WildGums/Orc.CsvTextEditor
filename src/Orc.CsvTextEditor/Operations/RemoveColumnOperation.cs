// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveColumnOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    public class RemoveColumnOperation : OperationBase
    {
        #region Constructors
        public RemoveColumnOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            var location = CsvTextEditorInstance.GetLocation();

            var text = CsvTextEditorInstance.GetText();
            text = text.RemoveCommaSeparatedColumn(location.Column.Index, CsvTextEditorInstance.LinesCount, CsvTextEditorInstance.ColumnsCount, CsvTextEditorInstance.LineEnding);

            CsvTextEditorInstance.SetText(text);
            CsvTextEditorInstance.GotoPosition(location.Line.Index, location.Column.Index);
        }
        #endregion
    }
}
