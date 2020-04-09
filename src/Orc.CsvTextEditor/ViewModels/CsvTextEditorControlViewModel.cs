// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextEditorControlViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
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
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IServiceLocator _serviceLocator;
        private readonly ICsvTextSynchronizationService _csvTextSynchronizationService;
        #endregion

        #region Constructors
        public CsvTextEditorControlViewModel(IServiceLocator serviceLocator, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => serviceLocator);
            Argument.IsNotNull(() => typeFactory);

            _serviceLocator = serviceLocator;
            _csvTextSynchronizationService = typeFactory.CreateInstanceWithParametersAndAutoCompletion<CsvTextSynchronizationService>();
            _serviceLocator.RegisterInstance(_csvTextSynchronizationService, this);

            Paste = new Command(() => CsvTextEditorInstance.Paste());
            Cut = new Command(() => CsvTextEditorInstance.Cut());

            GotoNextColumn = new Command(() => CsvTextEditorInstance.ExecuteOperation<GotoNextColumnOperation>());
            GotoPreviousColumn = new Command(() => CsvTextEditorInstance.ExecuteOperation<GotoPreviousColumnOperation>());

            Undo = new Command(() => CsvTextEditorInstance.Undo(), () => CsvTextEditorInstance.CanUndo);
            Redo = new Command(() => CsvTextEditorInstance.Redo(), () => CsvTextEditorInstance.CanRedo);

            AddLine = new Command(() => CsvTextEditorInstance.ExecuteOperation<AddLineOperation>());
            RemoveLine = new Command(() => CsvTextEditorInstance.ExecuteOperation<RemoveLineOperation>());
            DuplicateLine = new Command(() => CsvTextEditorInstance.ExecuteOperation<DuplicateLineOperation>());
            RemoveColumn = new Command(() => CsvTextEditorInstance.ExecuteOperation<RemoveColumnOperation>());
            AddColumn = new Command(() => CsvTextEditorInstance.ExecuteOperation<AddColumnOperation>());
            DeleteNextSelectedText = new Command(() => CsvTextEditorInstance.DeleteNextSelectedText());
            DeletePreviousSelectedText = new Command(() => CsvTextEditorInstance.DeletePreviousSelectedText());
        }
        #endregion

        #region Properties
        public object Scope { get; set; }
        public ICsvTextEditorInstance CsvTextEditorInstance { get; set; }
        public string Text { get; set; }

        public Command Paste { get; }
        public Command Cut { get; }
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
            if (CsvTextEditorInstance is null)
            {
                return;
            }

            UpdateInitialization();
        }

        private void OnCsvTextEditorInstanceChanged()
        {
            if (CsvTextEditorInstance is null)
            {
                return;
            }

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
                    CsvTextEditorInstance.Initialize(Text);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update initialization");
            }
        }

        #endregion
    }
}
