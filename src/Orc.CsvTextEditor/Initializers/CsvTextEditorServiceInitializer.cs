// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextEditorServiceInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using Catel;
    using ICSharpCode.AvalonEdit;

    public class CsvTextEditorInitializer : ICsvTextEditorInitializer
    {
        #region ICsvTextEditorInitializer Members
        public virtual void Initialize(TextEditor textEditor, ICsvTextEditorInstance csvTextEditorInstance)
        {
            Argument.IsNotNull(() => textEditor);
            Argument.IsNotNull(() => csvTextEditorInstance);

            //this place are reserved for CsvTextEditor initialization
        }
        #endregion
    }
}
