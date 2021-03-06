﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrimWhitespacesOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    using System.Linq;
    using Catel.Logging;

    public class TrimWhitespacesOperation : OperationBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        public TrimWhitespacesOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            Log.Debug("Trimming white spaces");

            var text = CsvTextEditorInstance.GetText();
            var lines = text.GetLines(out var newLineSymbol);

            CsvTextEditorInstance.SetText(string.Join(newLineSymbol, lines.Select(x => x.TrimCommaSeparatedValues())));
        }
        #endregion
    }
}
