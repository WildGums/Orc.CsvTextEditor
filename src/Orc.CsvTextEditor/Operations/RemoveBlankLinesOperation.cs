// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveBlankLinesOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    using System.Linq;
    using Catel.Logging;

    public class RemoveBlankLinesOperation : OperationBase
    {
        #region Constants
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        public RemoveBlankLinesOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            Log.Debug("Removing blank lines");

            var text = CsvTextEditorInstance.GetText();
            var lines = text.GetLines(out string newLineSymbol);

            CsvTextEditorInstance.SetText(string.Join(newLineSymbol, lines.Where(x => !x.IsEmptyCommaSeparatedLine())));
        }
        #endregion
    }
}
