// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextEditorToolBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using Catel;
    using Controls;
    using ICSharpCode.AvalonEdit;

    [ObsoleteEx(TreatAsErrorFromVersion = "3.1.0", RemoveInVersion = "4.0.0", Message = "Use ControlToolBase instead")]
    public abstract class CsvTextEditorToolBase : ControlToolBase
    {
        #region Constructors
        protected CsvTextEditorToolBase(TextEditor textEditor, ICsvTextEditorInstance csvTextEditorInstance)
        {
            Argument.IsNotNull(() => textEditor);
            Argument.IsNotNull(() => csvTextEditorInstance);

            TextEditor = textEditor;
            CsvTextEditorInstance = csvTextEditorInstance;
        }
        #endregion

        #region Properties
        protected TextEditor TextEditor { get; }
        protected ICsvTextEditorInstance CsvTextEditorInstance { get; }
        #endregion
    }
}
