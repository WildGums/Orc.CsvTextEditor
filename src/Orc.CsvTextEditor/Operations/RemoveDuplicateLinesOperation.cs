// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveDuplicateLinesOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    using Catel.Logging;

    public class RemoveDuplicateLinesOperation : OperationBase
    {
        #region Constants
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        public RemoveDuplicateLinesOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            Log.Debug("Removing duplicate lines");

            var text = _csvTextEditorInstance.GetText();
            var lines = text.GetLines(out string newLineSymbol);

            _csvTextEditorInstance.SetText(string.Join(newLineSymbol, lines));
        }
        #endregion
    }
}