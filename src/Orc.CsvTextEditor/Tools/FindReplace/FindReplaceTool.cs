namespace Orc.CsvTextEditor
{
    using System;
    using Catel.Services;
    using Controls;
    using ICSharpCode.AvalonEdit;
    using Microsoft.Extensions.DependencyInjection;

    public class FindReplaceTool : FindReplaceTool<FindReplaceService>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICsvTextEditorInstanceManager _csvTextEditorInstanceManager;

        public FindReplaceTool(IServiceProvider serviceProvider, IUIVisualizerService uiVisualizerService,
            ICsvTextEditorInstanceManager csvTextEditorInstanceManager)
            : base(serviceProvider, uiVisualizerService)
        {
            _serviceProvider = serviceProvider;
            _csvTextEditorInstanceManager = csvTextEditorInstanceManager;
        }

        protected override FindReplaceService? CreateFindReplaceService(object target)
        {
            if (target is not CsvTextEditorControl csvTextEditorControl)
            {
                return null;
            }

            var csvTextEditorInstance = _csvTextEditorInstanceManager.GetInstance(csvTextEditorControl.Id);
            if (csvTextEditorInstance is null)
            {
                return null;
            }

            if (csvTextEditorInstance.GetEditor() is not TextEditor textEditor)
            {
                return null;
            }

            var findReplaceService = ActivatorUtilities.CreateInstance<FindReplaceService>(_serviceProvider, textEditor, csvTextEditorInstance);

            return findReplaceService;
        }
    }
}
