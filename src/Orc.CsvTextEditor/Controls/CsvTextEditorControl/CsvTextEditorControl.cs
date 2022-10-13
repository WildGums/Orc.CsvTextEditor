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
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ITypeFactory _typeFactory;
        private readonly ICsvTextSynchronizationService _synchronizationService;

        private TextEditor? _textEditor;
        private bool _isPendingAttach = false;

        public CsvTextEditorControl()
        {
#pragma warning disable IDISP001 // Dispose created
            var serviceLocator = this.GetServiceLocator();
#pragma warning restore IDISP001 // Dispose created
            _typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();

            _synchronizationService = _typeFactory.CreateRequiredInstanceWithParametersAndAutoCompletion<CsvTextSynchronizationService>();
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
            CreateRoutedCommandBinding(QuoteColumn, () => CsvTextEditorInstance?.ExecuteOperation<QuoteColumnOperation>());

            CreateRoutedCommandBinding(DeleteNextSelectedText, () => CsvTextEditorInstance?.DeleteNextSelectedText());
            CreateRoutedCommandBinding(DeletePreviousSelectedText, () => CsvTextEditorInstance?.DeletePreviousSelectedText());
        }

        public static RoutedCommand Paste { get; } = new(nameof(Paste), typeof(CsvTextEditorControl));
        public static RoutedCommand Cut { get; } = new(nameof(Cut), typeof(CsvTextEditorControl));
        public static RoutedCommand Copy { get; } = new(nameof(Copy), typeof(CsvTextEditorControl));

        public static RoutedCommand GotoNextColumn { get; } = new(nameof(GotoNextColumn), typeof(CsvTextEditorControl));
        public static RoutedCommand GotoPreviousColumn { get; } = new(nameof(GotoPreviousColumn), typeof(CsvTextEditorControl));

        public static RoutedCommand Undo { get; } = new(nameof(Undo), typeof(CsvTextEditorControl));
        public static RoutedCommand Redo { get; } = new(nameof(Redo), typeof(CsvTextEditorControl));

        public static RoutedCommand AddLine { get; } = new(nameof(AddLine), typeof(CsvTextEditorControl));
        public static RoutedCommand RemoveLine { get; } = new(nameof(RemoveLine), typeof(CsvTextEditorControl));
        public static RoutedCommand DuplicateLine { get; } = new(nameof(DuplicateLine), typeof(CsvTextEditorControl));
        public static RoutedCommand RemoveColumn { get; } = new(nameof(RemoveColumn), typeof(CsvTextEditorControl));
        public static RoutedCommand AddColumn { get; } = new(nameof(AddColumn), typeof(CsvTextEditorControl));
        public static RoutedCommand QuoteColumn { get; } = new(nameof(QuoteColumn), typeof(CsvTextEditorControl));

        public static RoutedCommand DeleteNextSelectedText { get; } = new(nameof(DeleteNextSelectedText), typeof(CsvTextEditorControl));
        public static RoutedCommand DeletePreviousSelectedText { get; } = new(nameof(DeletePreviousSelectedText), typeof(CsvTextEditorControl));
        
        /// <summary>
        /// Customize Type of wrapper used for underlying TextEditor
        /// </summary>
        public Type? EditorInstanceType
        {
            get { return (Type?)GetValue(EditorInstanceTypeProperty); }
            set { SetValue(EditorInstanceTypeProperty, value); }
        }

        public static readonly DependencyProperty EditorInstanceTypeProperty = DependencyProperty.Register(nameof(EditorInstanceType), typeof(Type), typeof(CsvTextEditorControl),
            new PropertyMetadata(typeof(CsvTextEditorInstance),
                (sender, e) => ((CsvTextEditorControl)sender).OnTextEditorWrapperChanged(e)));

        public string? Text
        {
            get => (string?)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text),
            typeof(string), typeof(CsvTextEditorControl), new PropertyMetadata(default(string),
                (sender, args) => ((CsvTextEditorControl)sender).OnTextChanged(args)));

        public ICsvTextEditorInstance? CsvTextEditorInstance
        {
            get { return (ICsvTextEditorInstance?)GetValue(CsvTextEditorInstanceProperty); }
            set { SetValue(CsvTextEditorInstanceProperty, value); }
        }

        public static readonly DependencyProperty CsvTextEditorInstanceProperty =
            DependencyProperty.Register(nameof(CsvTextEditorInstance), typeof(ICsvTextEditorInstance), typeof(CsvTextEditorControl),
                new PropertyMetadata((sender, args) => ((CsvTextEditorControl)sender).OnCsvTextEditorInstanceChanged(args)));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var textEditor = GetTemplateChild("PART_TextEditor") as TextEditor;
            if (textEditor is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Can't find template part 'PART_TextEditor'");
            }

            _textEditor = textEditor;
            _textEditor.TextChanged += OnTextEditorTextChanged;

            if (_isPendingAttach)
            {
                _isPendingAttach = false;

                AttachCsvTextEditorInstance();
            }
            else
            {
                UpdateServiceRegistration();
            }
        }

        private void OnTextEditorWrapperChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateServiceRegistration();
        }

        private void OnCsvTextEditorInstanceChanged(DependencyPropertyChangedEventArgs args)
        {
            var oldInstance = args.OldValue as ICsvTextEditorInstance;
            oldInstance?.DetachEditor();

            if (_textEditor is null)
            {
                _isPendingAttach = true;
            }
            else
            {
                AttachCsvTextEditorInstance();

                _isPendingAttach = false;
            }
        }

        private void AttachCsvTextEditorInstance()
        {
            if (_textEditor is not null)
            {
                CsvTextEditorInstance?.AttachEditor(_textEditor);
            }

            UpdateInitialization();
        }

        private void OnTextEditorTextChanged(object? sender, EventArgs e)
        {
            if (_synchronizationService?.IsSynchronizing ?? true)
            {
                return;
            }

            var textEditor = _textEditor;
            if (textEditor is null)
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

        private void UpdateServiceRegistration(bool forceCreate = true)
        {
            var wrapperInstanceType = EditorInstanceType;
            if (_textEditor is null || wrapperInstanceType is null)
            {
                return;
            }

            if (!typeof(ICsvTextEditorInstance).IsAssignableFrom(wrapperInstanceType))
            {
                Log.Error($"Cannot use type {wrapperInstanceType} because it not implemented ICsvTextEditorInstance");
            }

            if (CsvTextEditorInstance is null || forceCreate)
            {
                var csvTextEditorInstance = (ICsvTextEditorInstance)_typeFactory.CreateRequiredInstanceWithParametersAndAutoCompletion(wrapperInstanceType);
                SetCurrentValue(CsvTextEditorInstanceProperty, csvTextEditorInstance);
            }
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
                    CsvTextEditorInstance?.Initialize(Text ?? string.Empty);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update initialization");
            }
        }

        private void CreateRoutedCommandBinding(RoutedCommand routedCommand, Action executeAction, Func<bool>? canExecute = null)
        {
            var routedCommandBinding = new CommandBinding { Command = routedCommand };
            routedCommandBinding.Executed += (sender, args) => executeAction?.Invoke();

            if (canExecute is not null)
            {
                routedCommandBinding.CanExecute += (sender, args) => args.CanExecute = canExecute.Invoke();
            }

            CommandBindings.Add(routedCommandBinding);
        }
    }
}
