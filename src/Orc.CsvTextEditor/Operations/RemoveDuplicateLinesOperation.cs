namespace Orc.CsvTextEditor.Operations
{
    using Catel.Logging;
    using Microsoft.Extensions.Logging;

    public class RemoveDuplicateLinesOperation : OperationBase
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(RemoveDuplicateLinesOperation));

        public RemoveDuplicateLinesOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }

        public override void Execute()
        {
            Logger.LogDebug("Removing duplicate lines");

            var text = _csvTextEditorInstance.GetText();
            var lines = text.GetLines(out var newLineSymbol);

            _csvTextEditorInstance.SetText(string.Join(newLineSymbol, lines));
        }
    }
}
