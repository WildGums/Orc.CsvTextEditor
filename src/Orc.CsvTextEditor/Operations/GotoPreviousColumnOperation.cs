namespace Orc.CsvTextEditor.Operations
{
    public class GotoPreviousColumnOperation : OperationBase
    {
        public GotoPreviousColumnOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }

        public override void Execute()
        {
            var location = _csvTextEditorInstance.GetLocation();

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
                columnIndex = _csvTextEditorInstance.ColumnsCount - 1;
                lineIndex--;
            }
            else
            {
                columnIndex--;
            }

            _csvTextEditorInstance.GotoPosition(lineIndex, columnIndex);
        }
    }
}
