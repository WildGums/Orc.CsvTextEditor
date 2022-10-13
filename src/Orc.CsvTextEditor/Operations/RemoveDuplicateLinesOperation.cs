namespace Orc.CsvTextEditor.Operations
{
    using Catel.Logging;

    public class RemoveDuplicateLinesOperation : OperationBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public RemoveDuplicateLinesOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }

        public override void Execute()
        {
            Log.Debug("Removing duplicate lines");

            var text = _csvTextEditorInstance.GetText();
            var lines = text.GetLines(out var newLineSymbol);

            _csvTextEditorInstance.SetText(string.Join(newLineSymbol, lines));
        }
    }
}
