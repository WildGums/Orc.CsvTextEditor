namespace Orc.CsvTextEditor
{
    using System;
    using Catel;
    using Catel.IoC;
    using Catel.Services;
    using Controls;
    using ICSharpCode.AvalonEdit;

    public class FindReplaceTool : FindReplaceTool<FindReplaceService>
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly ITypeFactory _typeFactory;

        public FindReplaceTool(IUIVisualizerService uiVisualizerService, ITypeFactory typeFactory, IServiceLocator serviceLocator)
            : base(uiVisualizerService, typeFactory, serviceLocator)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);
            ArgumentNullException.ThrowIfNull(serviceLocator);

            _typeFactory = typeFactory;
            _serviceLocator = serviceLocator;
        }

        protected override FindReplaceService? CreateFindReplaceService(object target)
        {
            if (target is not CsvTextEditorControl csvTextEditorControl)
            {
                return null;
            }

            var csvTextEditorInstance = csvTextEditorControl.CsvTextEditorInstance;
            if (csvTextEditorInstance is null)
            {
                return null;
            }

            if (csvTextEditorInstance.GetEditor() is not TextEditor textEditor)
            {
                return null;
            }

            var findReplaceService = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<FindReplaceService>(textEditor, csvTextEditorInstance);

            return findReplaceService;
        }
    }
}
