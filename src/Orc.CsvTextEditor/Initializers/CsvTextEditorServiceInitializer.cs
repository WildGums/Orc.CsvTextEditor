// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextEditorServiceInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using Catel;
    using Catel.IoC;
    using ICSharpCode.AvalonEdit;

    public class CsvTextEditorInitializer : ICsvTextEditorInitializer
    {
        #region Fields
        private readonly ITypeFactory _typeFactory;
        #endregion

        #region Constructors
        public CsvTextEditorInitializer(ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => typeFactory);

            _typeFactory = typeFactory;
        }
        #endregion

        #region ICsvTextEditorInitializer Members
        public virtual void Initialize(TextEditor textEditor, ICsvTextEditorInstance csvTextEditorInstance)
        {
            Argument.IsNotNull(() => textEditor);
            Argument.IsNotNull(() => csvTextEditorInstance);

            var findReplaceTool = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<FindReplaceTextEditorTool>(textEditor, csvTextEditorInstance);

            csvTextEditorInstance.AddTool(findReplaceTool);
        }
        #endregion
    }
}
