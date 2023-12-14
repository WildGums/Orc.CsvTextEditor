namespace Orc.CsvTextEditor.Operations
{
    public class AddLineOperation : OperationBase
    {
        public AddLineOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }

        public override void Execute()
        {
            var location = _csvTextEditorInstance.GetLocation();

            var oldText = _csvTextEditorInstance.GetText();
            var text = oldText.InsertLineWithTextTransfer(location.Line.Index + 1, location.GetOffsetInLine(), _csvTextEditorInstance.ColumnsCount, _csvTextEditorInstance.LineEnding);

            _csvTextEditorInstance.SetText(text);
            _csvTextEditorInstance.GotoPosition(location.Line.Index + 1, 0);
        }
    }
}
