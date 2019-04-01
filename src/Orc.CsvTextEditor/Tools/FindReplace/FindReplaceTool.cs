// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindReplaceTool.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using Catel;
    using Catel.IoC;
    using Catel.Services;
    using Controls;

    public class FindReplaceTool : FindReplaceTool<FindReplaceService>
    {
        #region Fields
        private readonly IServiceLocator _serviceLocator;
        private readonly ITypeFactory _typeFactory;
        #endregion

        #region Constructors
        public FindReplaceTool(IUIVisualizerService uiVisualizerService, ITypeFactory typeFactory, IServiceLocator serviceLocator)
            : base(uiVisualizerService, typeFactory)
        {
            Argument.IsNotNull(() => typeFactory);
            Argument.IsNotNull(() => serviceLocator);

            _typeFactory = typeFactory;
            _serviceLocator = serviceLocator;
        }
        #endregion

        #region Methods
        protected override FindReplaceService CreateFindReplaceService(object target)
        {
            if (!(target is CsvTextEditorControl csvTextEditorControl))
            {
                return null;
            }

            var textEditor = csvTextEditorControl.TextEditor;
            var csvTextEditorInstance = _serviceLocator.ResolveType<ICsvTextEditorInstance>(csvTextEditorControl.Scope);
            csvTextEditorInstance.AddTool(this);

            var findReplaceService = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<FindReplaceService>(textEditor, csvTextEditorInstance);

            return findReplaceService;
        }

        public override void Detach()
        {
            if (!(Target is CsvTextEditorControl csvTextEditorControl))
            {
                return;
            }

            var textEditor = csvTextEditorControl.TextEditor;
            var csvTextEditorInstance = _serviceLocator.ResolveType<ICsvTextEditorInstance>(csvTextEditorControl.Scope);
            csvTextEditorInstance.RemoveTool(this);

            base.Detach();
        }
        #endregion
    }
}
