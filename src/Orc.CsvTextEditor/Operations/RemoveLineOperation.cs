namespace Orc.CsvTextEditor.Operations
{
    public class RemoveLineOperation : OperationBase
    {
        public RemoveLineOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }

        public override void Execute()
        {
            var location = _csvTextEditorInstance.GetLocation();

            var text = _csvTextEditorInstance.GetText();

            if (location.Line.Index == _csvTextEditorInstance.LinesCount - 1)
            {
                text = text.Remove(location.Line.Offset);
                text = text.TrimEnd();
            }
            else
            {
                text = text.Remove(location.Line.Offset, location.Line.Length + _csvTextEditorInstance.LineEnding.Length);
            }

            _csvTextEditorInstance.SetText(text);

            _csvTextEditorInstance.GotoPosition(location.Line.Index - 1, location.Column.Index);
        }
    }
}
