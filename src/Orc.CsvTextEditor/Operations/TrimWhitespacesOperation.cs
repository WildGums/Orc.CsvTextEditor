namespace Orc.CsvTextEditor.Operations
{
    using System.Linq;
    using Catel.Logging;
    using Microsoft.Extensions.Logging;

    public class TrimWhitespacesOperation : OperationBase
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(TrimWhitespacesOperation));

        public TrimWhitespacesOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }

        public override void Execute()
        {
            Logger.LogDebug("Trimming white spaces");

            var text = _csvTextEditorInstance.GetText();
            var lines = text.GetLines(out var newLineSymbol);

            _csvTextEditorInstance.SetText(string.Join(newLineSymbol, lines.Select(x => x.TrimCommaSeparatedValues())));
        }
    }
}
