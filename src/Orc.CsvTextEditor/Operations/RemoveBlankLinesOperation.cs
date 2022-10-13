namespace Orc.CsvTextEditor.Operations
{
    using System.Linq;
    using Catel.Logging;

    public class RemoveBlankLinesOperation : OperationBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public RemoveBlankLinesOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }

        public override void Execute()
        {
            Log.Debug("Removing blank lines");

            var text = _csvTextEditorInstance.GetText();
            var lines = text.GetLines(out string newLineSymbol);

            _csvTextEditorInstance.SetText(string.Join(newLineSymbol, lines.Where(x => !x.IsEmptyCommaSeparatedLine())));
        }
    }
}
