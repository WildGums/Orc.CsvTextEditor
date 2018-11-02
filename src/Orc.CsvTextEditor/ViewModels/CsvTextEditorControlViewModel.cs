// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextEditorControlViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using Operations;

    internal class CsvTextEditorControlViewModel : ViewModelBase
    {
        #region Constants
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly IServiceLocator _serviceLocator;
        private ICsvTextEditorInstance _csvTextEditorInstance;
        private ICsvTextSynchronizationService _csvTextSynchronizationService;
        #endregion

        #region Constructors
        public CsvTextEditorControlViewModel(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            Argument.IsNotNull(() => serviceLocator);

            Paste = new Command(() => _csvTextEditorInstance.Paste());
            Cut = new Command(() => _csvTextEditorInstance.Cut());

            GotoNextColumn = new Command(() => _csvTextEditorInstance.ExecuteOperation<GotoNextColumnOperation>());
            GotoPreviousColumn = new Command(() => _csvTextEditorInstance.ExecuteOperation<GotoPreviousColumnOperation>());

            Undo = new Command(() => _csvTextEditorInstance.Undo(), () => _csvTextEditorInstance.CanUndo);
            Redo = new Command(() => _csvTextEditorInstance.Redo(), () => _csvTextEditorInstance.CanRedo);

            AddLine = new Command(() => _csvTextEditorInstance.ExecuteOperation<AddLineOperation>());
            RemoveLine = new Command(() => _csvTextEditorInstance.ExecuteOperation<RemoveLineOperation>());
            DuplicateLine = new Command(() => _csvTextEditorInstance.ExecuteOperation<DuplicateLineOperation>());
            RemoveColumn = new Command(() => _csvTextEditorInstance.ExecuteOperation<RemoveColumnOperation>());
            AddColumn = new Command(() => _csvTextEditorInstance.ExecuteOperation<AddColumnOperation>());
            DeleteNextSelectedText = new Command(() => _csvTextEditorInstance.DeleteNextSelectedText());
            DeletePreviousSelectedText = new Command(() => _csvTextEditorInstance.DeletePreviousSelectedText());
        }
        #endregion

        #region Properties
        public object Scope { get; set; }
        public string Text { get; set; }

        public Command Paste { get; set; }
        public Command Cut { get; set; }

        public Command GotoNextColumn { get; }
        public Command GotoPreviousColumn { get; }
        public Command Redo { get; }
        public Command Undo { get; }
        public Command ShowFindReplaceDialog { get; }
        public Command DeletePreviousSelectedText { get; }
        public Command DeleteNextSelectedText { get; }
        public Command AddColumn { get; }
        public Command RemoveColumn { get; }
        public Command DuplicateLine { get; }
        public Command RemoveLine { get; }
        public Command AddLine { get; }
        #endregion

        #region Methods
        private void OnTextChanged()
        {
            UpdateInitialization();
        }

        private void UpdateInitialization()
        {
            try
            {
                if (_csvTextSynchronizationService?.IsSynchronizing ?? true)
                {
                    return;
                }

                using (_csvTextSynchronizationService.SynchronizeInScope())
                {
                    _csvTextEditorInstance.Initialize(Text);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update initialization");
            }
        }

        private void OnScopeChanged()
        {
            var scope = Scope;

            if (scope == null)
            {
                _csvTextEditorInstance = null;
                _csvTextSynchronizationService = null;

                return;
            }

            if (_csvTextEditorInstance == null && _serviceLocator.IsTypeRegistered<ICsvTextEditorInstance>(scope))
            {
                _csvTextEditorInstance = _serviceLocator.ResolveType<ICsvTextEditorInstance>(scope);
            }

            if (_csvTextSynchronizationService == null && _serviceLocator.IsTypeRegistered<ICsvTextSynchronizationService>(scope))
            {
                _csvTextSynchronizationService = _serviceLocator.ResolveType<ICsvTextSynchronizationService>(scope);
            }

            UpdateInitialization();
        }
        #endregion
    }
}
