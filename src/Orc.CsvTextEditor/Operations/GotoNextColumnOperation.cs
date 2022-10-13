namespace Orc.CsvTextEditor.Operations
{
    public class GotoNextColumnOperation : OperationBase
    {
        public GotoNextColumnOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
 
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
    }
}
