namespace Orc.CsvTextEditor.Operations
{
    public class RemoveColumnOperation : OperationBase
    {
        public RemoveColumnOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }

        public override void Execute()
        {
            var location = _csvTextEditorInstance.GetLocation();

            var text = _csvTextEditorInstance.GetText();
            text = text.RemoveCommaSeparatedColumn(location.Column.Index, _csvTextEditorInstance.LinesCount, _csvTextEditorInstance.ColumnsCount, _csvTextEditorInstance.LineEnding);

            _csvTextEditorInstance.SetText(text);
            _csvTextEditorInstance.GotoPosition(location.Line.Index, location.Column.Index);
        }
    }
}
