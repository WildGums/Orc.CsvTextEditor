namespace Orc.CsvTextEditor.Operations
{
    using System.Linq;
    using Catel.Logging;

    public class TrimWhitespacesOperation : OperationBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public TrimWhitespacesOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }

        public override void Execute()
        {
            Log.Debug("Trimming white spaces");

            var text = _csvTextEditorInstance.GetText();
            var lines = text.GetLines(out var newLineSymbol);

            _csvTextEditorInstance.SetText(string.Join(newLineSymbol, lines.Select(x => x.TrimCommaSeparatedValues())));
        }
    }
}
