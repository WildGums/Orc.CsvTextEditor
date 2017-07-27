// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICsvTextEditorInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using ICSharpCode.AvalonEdit;

    public interface ICsvTextEditorInitializer
    {
        void Initialize(TextEditor textEditor, ICsvTextEditorInstance textEditorInstance);
    }
}