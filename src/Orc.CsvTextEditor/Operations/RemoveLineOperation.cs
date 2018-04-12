// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveLineOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    public class RemoveLineOperation : OperationBase
    {
        #region Constructors
        public RemoveLineOperation(ICsvTextEditorInstance csvTextEditorInstance)
            : base(csvTextEditorInstance)
        {
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            var location = _csvTextEditorInstance.GetLocation();

            var text = _csvTextEditorInstance.GetText();
            text = text.Remove(location.Line.Offset, location.Line.Length + _csvTextEditorInstance.LineEnding.Length);

            _csvTextEditorInstance.SetText(text);

            _csvTextEditorInstance.GotoPosition(location.Line.Index - 1, location.Column.Index);
        }
        #endregion
    }
}