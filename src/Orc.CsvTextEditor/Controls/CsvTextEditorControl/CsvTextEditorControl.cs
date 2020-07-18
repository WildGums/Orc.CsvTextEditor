// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextEditorControl.cs" company="WildGums">
//   Copyright (c) 2008 - 2020 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Catel.IoC;
    using Catel.Logging;
    using Controls;
    using ICSharpCode.AvalonEdit;
    using Operations;

    [TemplatePart(Name = "PART_TextEditor", Type = typeof(TextEditor))]
    public class CsvTextEditorControl : Control
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ITypeFactory _typeFactory;
        private readonly ICsvTextSynchronizationService _synchronizationService;

        private TextEditor _textEditor;
        #endregion

        #region Constructors
        public CsvTextEditorControl()
        {
            var serviceLocator = this.GetServiceLocator();
            _typeFactory = serviceLocator.ResolveType<ITypeFactory>();

            _synchronizationService = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<CsvTextSynchronizationService>();
            serviceLocator.RegisterInstance(_synchronizationService, this);

            CreateRoutedCommandBinding(Paste, () => CsvTextEditorInstance?.Paste());
            CreateRoutedCommandBinding(Cut, () => CsvTextEditorInstance?.Cut());
            CreateRoutedCommandBinding(Copy, () => CsvTextEditorInstance?.Copy());

            CreateRoutedCommandBinding(GotoNextColumn, () => CsvTextEditorInstance?.ExecuteOperation<GotoNextColumnOperation>());
            CreateRoutedCommandBinding(GotoPreviousColumn, () => CsvTextEditorInstance?.ExecuteOperation<GotoPreviousColumnOperation>());
           
            CreateRoutedCommandBinding(Undo, () => CsvTextEditorInstance?.Undo(), () => CsvTextEditorInstance?.CanUndo == true);
            CreateRoutedCommandBinding(Redo, () => CsvTextEditorInstance?.Redo(), () => CsvTextEditorInstance?.CanRedo == true);

            CreateRoutedCommandBinding(AddLine, () => CsvTextEditorInstance?.ExecuteOperation<AddLineOperation>());
            CreateRoutedCommandBinding(RemoveLine, () => CsvTextEditorInstance?.ExecuteOperation<RemoveLineOperation>());
            CreateRoutedCommandBinding(DuplicateLine, () => CsvTextEditorInstance?.ExecuteOperation<DuplicateLineOperation>());
            CreateRoutedCommandBinding(RemoveColumn, () => CsvTextEditorInstance?.ExecuteOperation<RemoveColumnOperation>());
            CreateRoutedCommandBinding(AddColumn, () => CsvTextEditorInstance?.ExecuteOperation<AddColumnOperation>());

            CreateRoutedCommandBinding(DeleteNextSelectedText, () => CsvTextEditorInstance?.DeleteNextSelectedText());
            CreateRoutedCommandBinding(DeletePreviousSelectedText, () => CsvTextEditorInstance?.DeletePreviousSelectedText());
        }
        #endregion

        #region Routed commands
        public static RoutedCommand Paste { get; } = new RoutedCommand(nameof(Paste), typeof(CsvTextEditorControl));
        public static RoutedCommand Cut { get; } = new RoutedCommand(nameof(Cut), typeof(CsvTextEditorControl));
        public static RoutedCommand Copy { get; } = new RoutedCommand(nameof(Copy), typeof(CsvTextEditorControl));
        
        public static RoutedCommand GotoNextColumn { get; } = new RoutedCommand(nameof(GotoNextColumn), typeof(CsvTextEditorControl));
        public static RoutedCommand GotoPreviousColumn { get; } = new RoutedCommand(nameof(GotoPreviousColumn), typeof(CsvTextEditorControl));
       
        public static RoutedCommand Undo { get; } = new RoutedCommand(nameof(Undo), typeof(CsvTextEditorControl));
        public static RoutedCommand Redo { get; } = new RoutedCommand(nameof(Redo), typeof(CsvTextEditorControl));

        public static RoutedCommand AddLine { get; } = new RoutedCommand(nameof(AddLine), typeof(CsvTextEditorControl));
        public static RoutedCommand RemoveLine { get; } = new RoutedCommand(nameof(RemoveLine), typeof(CsvTextEditorControl));
        public static RoutedCommand DuplicateLine { get; } = new RoutedCommand(nameof(DuplicateLine), typeof(CsvTextEditorControl));
        public static RoutedCommand RemoveColumn { get; } = new RoutedCommand(nameof(RemoveColumn), typeof(CsvTextEditorControl));
        public static RoutedCommand AddColumn { get; } = new RoutedCommand(nameof(AddColumn), typeof(CsvTextEditorControl));

        public static RoutedCommand DeleteNextSelectedText { get; } = new RoutedCommand(nameof(DeleteNextSelectedText), typeof(CsvTextEditorControl));
        public static RoutedCommand DeletePreviousSelectedText { get; } = new RoutedCommand(nameof(DeletePreviousSelectedText), typeof(CsvTextEditorControl));
        #endregion

        #region Dependency properties
        /// <summary>
        /// Customize Type of wrapper used for underlying TextEditor
        /// </summary>
        public Type EditorInstanceType
        {
            get { return (Type)GetValue(EditorInstanceTypeProperty); }
            set { SetValue(EditorInstanceTypeProperty, value); }
        }

        public static readonly DependencyProperty EditorInstanceTypeProperty = DependencyProperty.Register(nameof(EditorInstanceType), typeof(Type), typeof(CsvTextEditorControl),
            new PropertyMetadata(typeof(CsvTextEditorInstance),
                (sender, e) => ((CsvTextEditorControl)sender).OnTextEditorWrapperChanged(e)));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text),
            typeof(string), typeof(CsvTextEditorControl), new PropertyMetadata(default(string),
                (sender, args) => ((CsvTextEditorControl)sender).OnTextChanged(args)));

        public ICsvTextEditorInstance CsvTextEditorInstance
        {
            get { return (ICsvTextEditorInstance)GetValue(CsvTextEditorInstanceProperty); }
            set { SetValue(CsvTextEditorInstanceProperty, value); }
        }

        public static readonly DependencyProperty CsvTextEditorInstanceProperty =
            DependencyProperty.Register(nameof(CsvTextEditorInstance), typeof(ICsvTextEditorInstance), typeof(CsvTextEditorControl),
                new PropertyMetadata((sender, args) => ((CsvTextEditorControl)sender).OnCsvTextEditorInstanceChanged(args)));
        #endregion

        #region Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textEditor = GetTemplateChild("PART_TextEditor") as TextEditor;
            if (_textEditor is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Can't find template part 'PART_TextEditor'");
            }
            _textEditor.TextChanged += OnTextEditorTextChanged;

            UpdateServiceRegistration();
        }

        private void OnTextEditorWrapperChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateServiceRegistration();
        }

        private void OnCsvTextEditorInstanceChanged(DependencyPropertyChangedEventArgs args)
        {
            var oldInstance = args.OldValue as ICsvTextEditorInstance;
            oldInstance?.DetachEditor();

            var newInstance = args.NewValue as ICsvTextEditorInstance;
            newInstance?.AttachEditor(_textEditor);

            UpdateInitialization();
        }
        
        private void OnTextEditorTextChanged(object sender, EventArgs e)
        {
            if (_synchronizationService?.IsSynchronizing ?? true)
            {
                return;
            }

            var textEditor = _textEditor;
            if (textEditor == null)
            {
                return;
            }

            using (_synchronizationService.SynchronizeInScope())
            {
                SetCurrentValue(TextProperty, textEditor.Text);
            }
        }

        private void OnTextChanged(DependencyPropertyChangedEventArgs args)
        {
            if (CsvTextEditorInstance is null)
            {
                return;
            }

            UpdateInitialization();
        }

        private void UpdateServiceRegistration()
        {
            var wrapperInstanceType = EditorInstanceType;
            if (_textEditor == null || wrapperInstanceType == null)
            {
                return;
            }

            if (!typeof(ICsvTextEditorInstance).IsAssignableFrom(wrapperInstanceType))
            {
                Log.Error($"Cannot use type {wrapperInstanceType} because it not implemented ICsvTextEditorInstance");
            }

            var csvTextEditorInstance = (ICsvTextEditorInstance)_typeFactory.CreateInstanceWithParametersAndAutoCompletion(wrapperInstanceType);
            SetCurrentValue(CsvTextEditorInstanceProperty, csvTextEditorInstance);
        }

        private void UpdateInitialization()
        {
            try
            {
                if (_synchronizationService?.IsSynchronizing ?? true)
                {
                    return;
                }

                using (_synchronizationService.SynchronizeInScope())
                {
                    CsvTextEditorInstance?.Initialize(Text);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update initialization");
            }
        }

        private void CreateRoutedCommandBinding(RoutedCommand routedCommand, Action executeAction, Func<bool> canExecute = null)
        {
            var routedCommandBinding = new CommandBinding { Command = routedCommand };
            routedCommandBinding.Executed += (sender, args) => executeAction?.Invoke();

            if (canExecute != null)
            {
                routedCommandBinding.CanExecute += (sender, args) => args.CanExecute = canExecute.Invoke();
            }

            CommandBindings.Add(routedCommandBinding);
        }
        #endregion
    }
}
