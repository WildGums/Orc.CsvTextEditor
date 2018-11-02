// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICsvTextEditorInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using ICSharpCode.AvalonEdit;

    public interface ICsvTextEditorInitializer
    {
        #region Methods
        void Initialize(TextEditor textEditor, ICsvTextEditorInstance csvTextEditorInstance);
        #endregion
    }
}
