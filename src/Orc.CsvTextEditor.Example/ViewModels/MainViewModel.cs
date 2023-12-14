namespace Orc.CsvTextEditor.ViewModels
{
    using System;
    using Catel;
    using Catel.IoC;
    using Catel.MVVM;
    using ICSharpCode.AvalonEdit;
    using Orc.CsvTextEditor;

    public class MainViewModel : ViewModelBase
    {
        private readonly IServiceLocator _serviceLocator;

        public MainViewModel(IServiceLocator serviceLocator)//, ICsvTextEditorInstance csvTextEditorInstance)
        {
            ArgumentNullException.ThrowIfNull(serviceLocator);

            _serviceLocator = serviceLocator;

            EditorInstanceType = typeof(CsvTextEditorInstance);
            Title = "Orc.CsvTextEditor example";

            FindAndReplace = new Command(OnFindAndReplace);
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
