// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GotoNextColumnOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    public class GotoNextColumnOperation : OperationBase
    {
        #region Constructors
        public GotoNextColumnOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            var location = _csvTextEditorInstance.GetLocation();

            var columnIndex = location.Column.Index;
            var lineIndex = location.Line.Index;

            var isLastColumn = columnIndex + 1 == _csvTextEditorInstance.ColumnsCount;
            var isLastLine = lineIndex + 1 == _csvTextEditorInstance.LinesCount;

            if (isLastColumn && isLastLine)
            {
                return;
            }

            if (isLastColumn)
            {
                columnIndex = 0;
                lineIndex++;
            }
            else
            {
                columnIndex++;
            }

            _csvTextEditorInstance.GotoPosition(lineIndex, columnIndex);
        }
        #endregion
    }
}
