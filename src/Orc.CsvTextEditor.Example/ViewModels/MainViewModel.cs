namespace Orc.CsvTextEditor.ViewModels
{
    using System;
    using Catel.MVVM;
    using Orc.CsvTextEditor;

    public class MainViewModel : ViewModelBase
    {
        private readonly ICsvTextEditorInstanceManager _csvTextEditorInstanceManager;

        public MainViewModel(IServiceProvider serviceProvider, ICsvTextEditorInstanceManager csvTextEditorInstanceManager)
            : base(serviceProvider)
        {
            _csvTextEditorInstanceManager = csvTextEditorInstanceManager;

            Title = "Orc.CsvTextEditor example";

            FindAndReplace = new Command(serviceProvider, OnFindAndReplace);
        }

        public Command FindAndReplace { get; }

        public string EditorId { get; set; }

        private void OnFindAndReplace()
        {
#pragma warning disable IDISP001 // Dispose created
            var csvTextEditorInstance = _csvTextEditorInstanceManager.GetInstance(EditorId);
#pragma warning restore IDISP001 // Dispose created
            csvTextEditorInstance?.ShowTool<FindReplaceTool>();
        }
    }
}
