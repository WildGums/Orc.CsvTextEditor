// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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

        #region Constructors
        public MainViewModel(IServiceLocator serviceLocator)//, ICsvTextEditorInstance csvTextEditorInstance)
        {
            Argument.IsNotNull(() => serviceLocator);
            _serviceLocator = serviceLocator;

            EditorInstanceType = typeof(CsvTextEditorInstance);
            Title = "Orc.CsvTextEditor example";

            FindAndReplace = new Command(OnFindAndReplace);
        }

        #endregion

        #region Properties
        public Command FindAndReplace { get; }
        public Type EditorInstanceType { get; private set; }
        #endregion

        #region Methods
        private void OnFindAndReplace()
        {
            //var csvTextEditorInstance = _serviceLocator.TryResolveType<ICsvTextEditorInstance>(Scope);
            //csvTextEditorInstance?.ShowTool<FindReplaceTool>();
        }
        #endregion
    }
}
