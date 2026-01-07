namespace Orc.CsvTextEditor.Views
{
    using Orc.CsvTextEditor.ViewModels;

    public partial class MainWindow
    {
        protected override void OnViewModelChanged()
        {
            base.OnViewModelChanged();

            var vm = ViewModel as MainViewModel;
            if (vm is not null)
            {
                vm.EditorId = editor.Id; 
            }
        }
    }
}
