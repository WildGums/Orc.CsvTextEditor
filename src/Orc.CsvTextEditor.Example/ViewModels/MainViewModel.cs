namespace Orc.CsvTextEditor.ViewModels
{
    using System;
    using System.Threading.Tasks;
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

            FindAndReplace = new TaskCommand(serviceProvider, OnFindAndReplaceAsync);
        }

        public TaskCommand FindAndReplace { get; }

        public string EditorId { get; set; }

        private async Task OnFindAndReplaceAsync()
        {
#pragma warning disable IDISP001 // Dispose created
            var csvTextEditorInstance = _csvTextEditorInstanceManager.GetInstance(EditorId);
#pragma warning restore IDISP001 // Dispose created
            await csvTextEditorInstance?.ShowToolAsync<FindReplaceTool>();
        }
    }
}
