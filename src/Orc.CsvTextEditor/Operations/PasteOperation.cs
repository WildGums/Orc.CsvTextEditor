namespace Orc.CsvTextEditor.Operations
{
    using System.Windows;

    public class PasteOperation : OperationBase
    {
        public PasteOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }

        public override void Execute()
        {
            var text = Clipboard.GetText();

            var csvTextEditorInstance = _csvTextEditorInstance;
            var lineEnding = _csvTextEditorInstance.LineEnding;

            var documentText = csvTextEditorInstance.GetText();
            var selectionStart = csvTextEditorInstance.SelectionStart;
            var selectionLength = csvTextEditorInstance.SelectionLength;

            documentText = documentText.Remove(selectionStart, selectionLength)
                .Insert(selectionStart, text);

            if (selectionLength > 0)
            {
                text += lineEnding;
            }

            csvTextEditorInstance.SetText(documentText);
            csvTextEditorInstance.GotoPosition(selectionStart + text.Length);
        }
    }
}
