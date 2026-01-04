namespace Orc.CsvTextEditor.Operations
{
    using System.Linq;
    using Catel.Logging;
    using Microsoft.Extensions.Logging;

    public class RemoveBlankLinesOperation : OperationBase
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(RemoveBlankLinesOperation));

        public RemoveBlankLinesOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }

        public override void Execute()
        {
            Logger.LogDebug("Removing blank lines");

            var text = _csvTextEditorInstance.GetText();
            var lines = text.GetLines(out string newLineSymbol);

            _csvTextEditorInstance.SetText(string.Join(newLineSymbol, lines.Where(x => !x.IsEmptyCommaSeparatedLine())));
        }
    }
}
