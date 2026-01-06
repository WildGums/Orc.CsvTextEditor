namespace Orc.CsvTextEditor.ViewModels
{
    using System;
    using Catel.MVVM;
    using Orc.CsvTextEditor;

    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            EditorInstanceType = typeof(CsvTextEditorInstance);
            Title = "Orc.CsvTextEditor example";

            FindAndReplace = new Command(serviceProvider, OnFindAndReplace);
        }

        public Command FindAndReplace { get; }
        public Type EditorInstanceType { get; private set; }

        private void OnFindAndReplace()
        {
            //var csvTextEditorInstance = _serviceLocator.TryResolveType<ICsvTextEditorInstance>(Scope);
            //csvTextEditorInstance?.ShowTool<FindReplaceTool>();
        }
    }
}
