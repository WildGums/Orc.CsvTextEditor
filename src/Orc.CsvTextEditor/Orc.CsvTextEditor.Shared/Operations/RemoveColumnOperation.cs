// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveColumnOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
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
            var location = _csvTextEditorInstance.GetLocation();

            var text = _csvTextEditorInstance.GetText();
            text = text.RemoveCommaSeparatedColumn(location.Column.Index, _csvTextEditorInstance.LinesCount, _csvTextEditorInstance.ColumnsCount, _csvTextEditorInstance.LineEnding);

            _csvTextEditorInstance.SetText(text);
            _csvTextEditorInstance.GotoPosition(location.Line.Index, location.Column.Index);
        }
        #endregion
    }
}