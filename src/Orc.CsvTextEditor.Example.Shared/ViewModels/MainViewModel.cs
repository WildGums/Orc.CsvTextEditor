// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.ViewModels
{
    using Catel;
    using Catel.IoC;
    using Catel.MVVM;

    public class MainViewModel : ViewModelBase
    {
        private readonly IServiceLocator _serviceLocator;

        #region Constructors
        public MainViewModel(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

            _serviceLocator = serviceLocator;

            Title = "Orc.CsvTextEditor example";

            FindAndReplace = new Command(OnFindAndReplace);
        }
        #endregion

        #region Properties
        #region Commands
        public Command FindAndReplace { get; }
        public object Scope => "Test_CsvTextEditor";
        #endregion
        #endregion

        #region Methods
        private void OnFindAndReplace()
        {
            var csvTextEditorInstance = _serviceLocator.TryResolveType<ICsvTextEditorInstance>(Scope);
            csvTextEditorInstance?.ShowTool<FindReplaceTextEditorTool>();
        }
        #endregion
    }
}
