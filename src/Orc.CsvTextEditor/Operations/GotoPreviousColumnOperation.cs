// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GotoPreviousColumnOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    public class GotoPreviousColumnOperation : OperationBase
    {
        #region Constructors
        public GotoPreviousColumnOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            var location = CsvTextEditorInstance.GetLocation();

            var columnIndex = location.Column.Index;
            var lineIndex = location.Line.Index;

            var isFirstColumn = columnIndex == 0;
            var isFirstLine = lineIndex == 0;

            if (isFirstColumn && isFirstLine)
            {
                return;
            }

            if (isFirstColumn)
            {
                columnIndex = CsvTextEditorInstance.ColumnsCount - 1;
                lineIndex--;
            }
            else
            {
                columnIndex--;
            }

            CsvTextEditorInstance.GotoPosition(lineIndex, columnIndex);
        }
        #endregion
    }
}
