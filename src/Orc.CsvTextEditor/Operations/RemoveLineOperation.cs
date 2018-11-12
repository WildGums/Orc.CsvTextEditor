// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveLineOperation.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
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
            var location = CsvTextEditorInstance.GetLocation();

            var text = CsvTextEditorInstance.GetText();

            if (location.Line.Index == CsvTextEditorInstance.LinesCount - 1)
            {
                text = text.Remove(location.Line.Offset);
                text = text.TrimEnd();
            }
            else
            {
                text = text.Remove(location.Line.Offset, location.Line.Length + CsvTextEditorInstance.LineEnding.Length);
            }

            CsvTextEditorInstance.SetText(text);

            CsvTextEditorInstance.GotoPosition(location.Line.Index - 1, location.Column.Index);
        }
        #endregion
    }
}
