namespace Orc.CsvTextEditor
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Catel.Logging;
    using ICSharpCode.AvalonEdit;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Operations;

    [TemplatePart(Name = "PART_TextEditor", Type = typeof(TextEditor))]
    public partial class CsvTextEditorControl : Control
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(CsvTextEditorControl));

        private readonly ICsvTextSynchronizationService _csvTextSynchronizationService;
        private readonly ICsvTextEditorInstanceManager _csvTextEditInstanceManager;
        private readonly IServiceProvider _serviceProvider;

        private TextEditor? _textEditor;

        private ICsvTextEditorInstance? _csvTextEditorInstance;

        public CsvTextEditorControl(ICsvTextSynchronizationService csvTextSynchronizationService,
            ICsvTextEditorInstanceManager csvTextEditInstanceManager, IServiceProvider serviceProvider)
        {
            _csvTextSynchronizationService = csvTextSynchronizationService;
            _csvTextEditInstanceManager = csvTextEditInstanceManager;
            _serviceProvider = serviceProvider;
            CreateRoutedCommandBinding(Paste, () => _csvTextEditorInstance?.Paste());
            CreateRoutedCommandBinding(Cut, () => _csvTextEditorInstance?.Cut());
            CreateRoutedCommandBinding(Copy, () => _csvTextEditorInstance?.Copy());

            CreateRoutedCommandBinding(GotoNextColumn, () => _csvTextEditorInstance?.ExecuteOperation<GotoNextColumnOperation>());
            CreateRoutedCommandBinding(GotoPreviousColumn, () => _csvTextEditorInstance?.ExecuteOperation<GotoPreviousColumnOperation>());

            CreateRoutedCommandBinding(Undo, () => _csvTextEditorInstance?.Undo(), () => _csvTextEditorInstance?.CanUndo == true);
            CreateRoutedCommandBinding(Redo, () => _csvTextEditorInstance?.Redo(), () => _csvTextEditorInstance?.CanRedo == true);

            CreateRoutedCommandBinding(AddLine, () => _csvTextEditorInstance?.ExecuteOperation<AddLineOperation>());
            CreateRoutedCommandBinding(RemoveLine, () => _csvTextEditorInstance?.ExecuteOperation<RemoveLineOperation>());
            CreateRoutedCommandBinding(DuplicateLine, () => _csvTextEditorInstance?.ExecuteOperation<DuplicateLineOperation>());
            CreateRoutedCommandBinding(RemoveColumn, () => _csvTextEditorInstance?.ExecuteOperation<RemoveColumnOperation>());
            CreateRoutedCommandBinding(AddColumn, () => _csvTextEditorInstance?.ExecuteOperation<AddColumnOperation>());
            CreateRoutedCommandBinding(QuoteColumn, () => _csvTextEditorInstance?.ExecuteOperation<QuoteColumnOperation>());

            CreateRoutedCommandBinding(DeleteNextSelectedText, () => _csvTextEditorInstance?.DeleteNextSelectedText());
            CreateRoutedCommandBinding(DeletePreviousSelectedText, () => _csvTextEditorInstance?.DeletePreviousSelectedText());

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
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

        public string Id { get { return _csvTextEditorInstance?.Id ?? string.Empty; } }

        public string? Text
        {
            get => (string?)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text),
            typeof(string), typeof(CsvTextEditorControl), new PropertyMetadata(default(string),
                (sender, args) => ((CsvTextEditorControl)sender).OnTextChanged(args)));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var textEditor = GetTemplateChild("PART_TextEditor") as TextEditor;
            if (textEditor is null)
            {
                throw Logger.LogErrorAndCreateException<InvalidOperationException>("Can't find template part 'PART_TextEditor'");
            }

            _textEditor = textEditor;
            _textEditor.TextChanged += OnTextEditorTextChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AttachCsvTextEditorInstance();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var textEditor = _textEditor;
            if (textEditor is not null)
            {
                textEditor.TextChanged -= OnTextEditorTextChanged;
            }

            DetachCsvTextEditorInstance();
        }

        private void AttachCsvTextEditorInstance()
        {
            _csvTextEditorInstance = ActivatorUtilities.CreateInstance<CsvTextEditorInstance>(_serviceProvider, _textEditor!);

            if (_textEditor is not null)
            {
                _csvTextEditorInstance.AttachEditor(_textEditor);
            }

            _csvTextEditInstanceManager.RegisterInstance(_csvTextEditorInstance.Id, _csvTextEditorInstance);

            UpdateInitialization();
        }

        private void DetachCsvTextEditorInstance()
        {
            var instance = _csvTextEditorInstance;
            if (instance is null)
            {
                return;
            }

            if (_textEditor is not null)
            {
                instance.DetachEditor();
            }

            _csvTextEditInstanceManager.UnregisterInstance(instance.Id);
        }

        private void OnTextEditorTextChanged(object? sender, EventArgs e)
        {
            if (_csvTextSynchronizationService.IsSynchronizing)
            {
                return;
            }

            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            using (_csvTextSynchronizationService.SynchronizeInScope())
            {
                SetCurrentValue(TextProperty, textEditor.Text);
            }
        }

        private void OnTextChanged(DependencyPropertyChangedEventArgs args)
        {
            if (_csvTextEditorInstance is null)
            {
                return;
            }

            UpdateInitialization();
        }

        private void UpdateInitialization()
        {
            try
            {
                if (_csvTextSynchronizationService.IsSynchronizing)
                {
                    return;
                }

                using (_csvTextSynchronizationService.SynchronizeInScope())
                {
                    _csvTextEditorInstance?.Initialize(Text ?? string.Empty);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to update initialization");
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
