namespace Orc.CsvTextEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using System.Xml;
    using Catel;
    using Catel.Collections;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Catel.Windows;
    using Controls;
    using Controls.Tools;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.CodeCompletion;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Operations;

    public class CsvTextEditorInstance : Disposable, ICsvTextEditorInstance
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(CsvTextEditorInstance));

        private static readonly string QuoteString = Symbols.Quote.ToString();

        private readonly ICommandManager _commandManager;
        private readonly IDispatcherService _dispatcherService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICsvTextEditorInitializer _initializer;
        private readonly DispatcherTimer _refreshViewTimer;

        private TabSpaceElementGenerator? _elementGenerator;
        private CompletionWindow? _completionWindow;
        private EditingState _editingState = EditingState.None;
        private HighlightAllOccurencesOfSelectedWordTransformer? _highlightAllOccurrencesOfSelectedWordTransformer;
        private FirstLineAlwaysBoldTransformer? _firstLineAlwaysBoldTransformer;
        private GrayedQuotesDocumentColorizingTransformer? _invisibleQuotesTransformer;
        private bool _initializing;
        private bool _isInCustomUpdate;
        private bool _isInRedoUndo;
        private Location? _lastLocation;
        private int _textChangingIterator;
        private bool _settingInitialText;

        private TextEditor? _textEditor;
        private CsvTextEditorControl? _csvTextEditorControl;

        public CsvTextEditorInstance(TextEditor textEditor, ICommandManager commandManager, ICsvTextEditorInitializer initializer,
            IDispatcherService dispatcherService, IServiceProvider serviceProvider)
            : this(commandManager, initializer, dispatcherService, serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(textEditor);

            AttachEditor(textEditor);
        }

        public CsvTextEditorInstance(ICommandManager commandManager, ICsvTextEditorInitializer initializer,
            IDispatcherService dispatcherService, IServiceProvider serviceProvider)
        {
            _commandManager = commandManager;
            _initializer = initializer;
            _dispatcherService = dispatcherService;
            _serviceProvider = serviceProvider;

            _refreshViewTimer = new DispatcherTimer();
            _refreshViewTimer.Tick += (sender, e) =>
            {
                if (sender is not DispatcherTimer timer)
                {
                    return;
                }

                timer.Stop();

                RefreshView();
            };
            _refreshViewTimer.Interval = TimeSpan.FromMilliseconds(50);
        }

        public IEnumerable<IControlTool> Tools => _csvTextEditorControl?.GetControlToolManager().Tools ?? new List<IControlTool>();
        public IControlToolManager ToolManager
        {
            get
            {
                var manager = _csvTextEditorControl?.GetControlToolManager();
                if (manager is null)
                {
                    throw Logger.LogErrorAndCreateException<InvalidOperationException>("Manager is null");
                }

                return manager;
            }
        }

        public int LinesCount => _textEditor?.Document?.LineCount ?? 0;
        public int ColumnsCount => _elementGenerator?.ColumnCount ?? 0;
        public bool IsAutocompleteEnabled { get; set; } = true;
        public bool HasSelection => _textEditor?.SelectionLength > 0;
        public bool CanRedo => !_initializing && (_textEditor?.CanRedo ?? false);
        public bool CanUndo => IsDirty && !_initializing && (_textEditor?.CanUndo ?? false);
        public string LineEnding => _elementGenerator?.NewLine ?? string.Empty;
        public bool IsDirty => _textChangingIterator != 0;
        public int SelectionStart => _textEditor?.SelectionStart ?? 0;
        public int SelectionLength => _textEditor?.SelectionLength ?? 0;
        public string SelectionText => _textEditor?.SelectedText ?? string.Empty;

        public void AttachEditor(object editor)
        {
            ArgumentNullException.ThrowIfNull(editor);

            DetachEditor();

            _textEditor = editor as TextEditor;
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            _csvTextEditorControl = textEditor.FindLogicalOrVisualAncestorByType<CsvTextEditorControl>();
            if (_csvTextEditorControl is null)
            {
                return;
            }

            // NOTE: Performance tips: https://github.com/icsharpcode/AvalonEdit/issues/11
            // Need to make these options accessible to the user in the settings window
            textEditor.SetCurrentValue(TextEditor.ShowLineNumbersProperty, true);
            textEditor.Options.HighlightCurrentLine = true;
            textEditor.Options.ShowEndOfLine = false;
            textEditor.Options.ShowTabs = false;
            textEditor.Options.ShowSpaces = false;
            textEditor.Options.EnableEmailHyperlinks = false;
            textEditor.Options.EnableHyperlinks = false;
            textEditor.Options.AllowScrollBelowDocument = false;

            _elementGenerator = ActivatorUtilities.CreateInstance<TabSpaceElementGenerator>(_serviceProvider);

            textEditor.TextArea.TextView.ElementGenerators.Add(_elementGenerator);

            textEditor.TextArea.SelectionChanged += OnTextAreaSelectionChanged;
            textEditor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
            textEditor.TextArea.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            textEditor.TextChanged += OnTextChanged;
            textEditor.PreviewKeyDown += OnPreviewKeyDown;

            textEditor.TextArea.TextEntering += OnTextEntering;

            _highlightAllOccurrencesOfSelectedWordTransformer = new HighlightAllOccurencesOfSelectedWordTransformer();
            textEditor.TextArea.TextView.LineTransformers.Add(_highlightAllOccurrencesOfSelectedWordTransformer);

            _firstLineAlwaysBoldTransformer = new FirstLineAlwaysBoldTransformer();
            textEditor.TextArea.TextView.LineTransformers.Add(_firstLineAlwaysBoldTransformer);

            _invisibleQuotesTransformer = new GrayedQuotesDocumentColorizingTransformer();
            textEditor.TextArea.TextView.LineTransformers.Add(_invisibleQuotesTransformer);

            _initializer.Initialize(textEditor, this);

            EditorAttached?.Invoke(this, EventArgs.Empty);
        }

        public void DetachEditor()
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            textEditor.TextArea.TextView.ElementGenerators.Remove(_elementGenerator);

            textEditor.TextArea.TextView.LineTransformers.Remove(_highlightAllOccurrencesOfSelectedWordTransformer);
            textEditor.TextArea.TextView.LineTransformers.Remove(_firstLineAlwaysBoldTransformer);

            textEditor.TextArea.SelectionChanged -= OnTextAreaSelectionChanged;
            textEditor.TextArea.Caret.PositionChanged -= OnCaretPositionChanged;
            textEditor.TextArea.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            textEditor.TextChanged -= OnTextChanged;
            textEditor.PreviewKeyDown -= OnPreviewKeyDown;
            textEditor.TextArea.TextEntering -= OnTextEntering;

            var document = textEditor.Document;
            document.Changed -= OnTextDocumentChanged;

            _textEditor = null;
            _highlightAllOccurrencesOfSelectedWordTransformer = null;

            EditorDetached?.Invoke(this, EventArgs.Empty);
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            var textEditor = _textEditor;
            if (textEditor is not null)
            {
                textEditor.TextArea.SelectionChanged -= OnTextAreaSelectionChanged;
                textEditor.TextArea.Caret.PositionChanged -= OnCaretPositionChanged;
                textEditor.TextArea.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
                textEditor.TextChanged -= OnTextChanged;
                textEditor.PreviewKeyDown -= OnPreviewKeyDown;

                textEditor.TextArea.TextEntering -= OnTextEntering;

                textEditor.TextArea.TextView.ElementGenerators.Clear();
                textEditor.TextArea.TextView.LineTransformers.Clear();
            }

            _refreshViewTimer.Stop();
        }

        private void TextEditorRedo()
        {
            _editingState = EditingState.Redoing;

            try
            {
                _textEditor?.Redo();
            }
            finally
            {
                _editingState = EditingState.Editing;
            }
        }

        private void TextEditorUndo()
        {
            _editingState = EditingState.Undoing;

            try
            {
                _textEditor?.Undo();
            }
            finally
            {
                _editingState = EditingState.Editing;
            }
        }

        private void OnTextEntering(object? sender, TextCompositionEventArgs e)
        {
            if (Equals(e.Text, QuoteString))
            {
                e.Handled = true;

                ExecuteOperation<QuoteColumnOperation>();

                return;
            }

            if (IsAutocompleteEnabled)
            {
                PerformAutoComplete(e.Text);
            }
        }

        private void PerformAutoComplete(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                _completionWindow?.Close();
                return;
            }

            if (_completionWindow is not null)
            {
                return;
            }

            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            var elementGenerator = _elementGenerator;
            if (elementGenerator is null)
            {
                return;
            }

            var columnIndex = GetCurrentColumnIndex();
            var data = textEditor.GetCompletionDataForText(inputText, columnIndex, elementGenerator.Lines);

            if (!data.Any())
            {
                return;
            }

            _completionWindow = new CompletionWindow(textEditor.TextArea);
            _completionWindow.CompletionList.CompletionData.AddRange(data);
            _completionWindow.Show();
            _completionWindow.Closed += (o, args) => _completionWindow = null;
        }

        private int GetCurrentColumnIndex()
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return 0;
            }

            var textDocument = textEditor.Document;
            var offset = textEditor.CaretOffset;
            var affectedLocation = textDocument.GetLocation(offset);

            var elementGenerator = _elementGenerator;
            if (elementGenerator is not null)
            {
                var columnNumberWithOffset = elementGenerator.GetColumn(affectedLocation);
                return columnNumberWithOffset?.Index ?? 0;
            }

            return affectedLocation.Column;
        }

        private void RefreshHighlightings()
        {
            using (var customHighlightings = GetType().Assembly.GetManifestResourceStream("Orc.CsvTextEditor.Resources.Highlightings.CustomHighlighting.xshd"))
            {
                if (customHighlightings is null)
                {
                    throw Logger.LogErrorAndCreateException<InvalidOperationException>("Could not find embedded resource");
                }

                using (var reader = new XmlTextReader(customHighlightings))
                {
                    _textEditor?.SetCurrentValue(TextEditor.SyntaxHighlightingProperty, HighlightingLoader.Load(reader, HighlightingManager.Instance));
                }
            }
        }

        private void RefreshLocation(int offset, int length)
        {
            if (_isInCustomUpdate || _isInRedoUndo)
            {
                return;
            }

            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            var textDocument = textEditor.Document;
            var affectedLocation = textDocument.GetLocation(offset);

            var elementGenerator = _elementGenerator;
            if (elementGenerator is null)
            {
                return;
            }

            if (elementGenerator.RefreshLocation(affectedLocation, length))
            {
                // Note: a redraw could be required here
            }
        }

        private void OnPreviewMouseLeftButtonDown(object? sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var elementGenerator = _elementGenerator;
            if (elementGenerator is null)
            {
                return;
            }

            if (elementGenerator.UnfreezeColumnResizing())
            {
                RefreshView();
            }
        }

        private void OnPreviewKeyDown(object? sender, KeyEventArgs e)
        {
            var elementGenerator = _elementGenerator;
            if (elementGenerator is null)
            {
                return;
            }

            if (e.Key == Key.Tab && elementGenerator.UnfreezeColumnResizing())
            {
                RefreshView();
            }
        }

        private void OnTextDocumentChanged(object? sender, DocumentChangeEventArgs e)
        {
            RefreshLocation(e.Offset, e.InsertionLength - e.RemovalLength);
        }

        private void ClearSelectedText()
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            var textDocument = textEditor.Document;
            var selectionStart = textEditor.SelectionStart;
            var selectionLength = textEditor.SelectionLength;

            if (selectionLength == 0)
            {
                return;
            }

            var text = textDocument.Text.Remove(selectionStart, selectionLength);

            text = text.RemoveEmptyLines();

            textEditor.SelectionLength = 0;
            UpdateText(text);

            textEditor.CaretOffset = text.Length < selectionStart
                ? text.Length
                : selectionStart;
        }

        private void DeleteFromPosition(int deletePosition)
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            var elementGenerator = _elementGenerator;
            if (elementGenerator is null)
            {
                return;
            }

            var textDocument = textEditor.Document;

            if (deletePosition < 0 || deletePosition >= textDocument.TextLength)
            {
                return;
            }

            var deletingChar = textDocument.Text[deletePosition];

            var textLocation = textDocument.GetLocation(deletePosition + 1);

            var column = elementGenerator.GetColumn(textLocation);
            if (column is null)
            {
                return;
            }

            if (deletingChar == Symbols.Quote)
            {
                char? charBefore = null;
                if (deletePosition > 0)
                {
                    charBefore = textDocument.Text[deletePosition - 1];
                }

                char? charAfter = null;
                if (deletePosition < textDocument.Text.Length - 2)
                {
                    charAfter = textDocument.Text[deletePosition + 1];
                }

                if (charAfter == Symbols.Comma || charBefore == Symbols.Comma)
                {
                    return;
                }
            }
            else if (column.Offset == textLocation.Column - 1)
            {
                return;
            }

            if (deletingChar == Symbols.NewLineStart || deletingChar == Symbols.NewLineEnd)
            {
                return;
            }

            textDocument.Remove(deletePosition, 1);
        }

        private void UpdateText(string text)
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            var elementGenerator = _elementGenerator;
            if (elementGenerator is null)
            {
                return;
            }

            elementGenerator.Refresh(text);

            _isInCustomUpdate = true;

            using (textEditor.Document.RunUpdate())
            {
                textEditor.Document.Text = text;
            }

            _isInCustomUpdate = false;
        }

        private static string AdjustText(string text)
        {
            text ??= string.Empty;

            var newLineSymbol = text.GetNewLineSymbol();
            return text.TrimEnd(newLineSymbol);
        }

        private void OnTextChanged(object? sender, EventArgs eventArgs)
        {
            UpdateTextChangingIterator();

            RaiseTextChanged();

            _refreshViewTimer.Stop();
            _refreshViewTimer.Start();
        }

        private void UpdateTextChangingIterator()
        {
            if (_settingInitialText)
            {
                ResetIsDirty();
                return;
            }

            switch (_editingState)
            {
                case EditingState.Undoing:
                    _textChangingIterator--;
                    break;

                case EditingState.Redoing:
                    _textChangingIterator++;
                    break;

                case EditingState.Editing:
                    _textChangingIterator++;
                    break;
            }

            _textEditor?.SetCurrentValue(TextEditor.IsModifiedProperty, _textChangingIterator == 0);
        }

        private void RaiseTextChanged()
        {
            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCaretPositionChanged(object? sender, EventArgs eventArgs)
        {
            var location = GetLocation();
            if (location is null)
            {
                return;
            }

            if (_lastLocation is not null && _lastLocation.Offset == location.Offset)
            {
                return;
            }

            CaretTextLocationChanged?.Invoke(this, new CaretTextLocationChangedEventArgs(location));

            _lastLocation = location;
        }

        private void OnTextAreaSelectionChanged(object? sender, EventArgs e)
        {
            _commandManager.InvalidateCommands();

            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            var highlightAllOccurrencesOfSelectedWordTransformer = _highlightAllOccurrencesOfSelectedWordTransformer;
            if (highlightAllOccurrencesOfSelectedWordTransformer is null)
            {
                return;
            }

            // Disable this line if the user is using the "Find Replace" dialog box
            highlightAllOccurrencesOfSelectedWordTransformer.SelectedWord = textEditor.SelectedText;
            highlightAllOccurrencesOfSelectedWordTransformer.Selection = textEditor.TextArea.Selection;

            textEditor.TextArea.TextView.Redraw();
        }

        private enum EditingState
        {
            None,
            Undoing,
            Redoing,
            Editing
        }

        public object GetEditor()
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                throw Logger.LogErrorAndCreateException<InvalidOperationException>("No editor has been registered");
            }

            return textEditor;
        }

        public string GetText()
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return string.Empty;
            }

            var text = string.Empty;

            _dispatcherService.Invoke(() => text = textEditor.Text);

            return text;
        }

        public void SetText(string text)
        {
            _dispatcherService.Invoke(() =>
            {
                text = AdjustText(text);

                UpdateText(text);
            });
        }

        public void SetInitialText(string text)
        {
            _settingInitialText = true;

            try
            {
                SetText(text);
            }
            finally
            {
                _settingInitialText = false;
            }
        }

        public void InsertAtPosition(int offset, string str)
        {
            _textEditor?.Document.Insert(offset, str);
        }

        public void ResetIsDirty()
        {
            _textChangingIterator = 0;
        }

        public void Copy()
        {
            _textEditor?.Copy();
        }

        public void Paste()
        {
            ExecuteOperation<PasteOperation>();
        }

        public void Redo()
        {
            using (new DisposableToken<CsvTextEditorInstance>(this, x => x.Instance._isInRedoUndo = true, x =>
            {
                RefreshView();
                x.Instance._isInRedoUndo = false;
            }))
            {
                TextEditorRedo();
            }
        }

        public void Undo()
        {
            using (new DisposableToken<CsvTextEditorInstance>(this, x => x.Instance._isInRedoUndo = true, x =>
            {
                RefreshView();
                x.Instance._isInRedoUndo = false;
            }))
            {
                TextEditorUndo();
            }
        }

        public void Cut()
        {
            var selectedText = _textEditor?.SelectedText ?? string.Empty;

            ClearSelectedText();

            Clipboard.SetText(selectedText);
        }

        public Location GetLocation()
        {
            var textEditor = _textEditor;
            var elementGenerator = _elementGenerator;
            if (textEditor is null || elementGenerator is null)
            {
                return new Location(new Column(), new Line());
            }

            var textDocument = textEditor.Document;
            var offset = textEditor.CaretOffset;
            var textLocation = textDocument.GetLocation(offset);

            var column = elementGenerator.GetColumn(textLocation);
            if (column is null)
            {
                return new Location(new Column(), new Line());
            }

            var lineIndex = textLocation.Line - 1;
            var documentLine = textDocument.Lines[lineIndex];

            var location = new Location(column, new Line
            {
                Index = lineIndex,
                Offset = documentLine.Offset,
                Length = documentLine.Length
            })
            {
                Offset = offset
            };

            return location;
        }

        public void ExecuteOperation<TOperation>() where TOperation : IOperation
        {
            var operation = ActivatorUtilities.CreateInstance<TOperation>(_serviceProvider, this);
            operation.Execute();
        }

        public void DeleteNextSelectedText()
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            var selectionLength = textEditor.SelectionLength;
            if (selectionLength == 0)
            {
                var deletePosition = textEditor.SelectionStart;
                DeleteFromPosition(deletePosition);
                return;
            }

            ClearSelectedText();
        }

        public void DeletePreviousSelectedText()
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            var selectionLength = textEditor.SelectionLength;
            if (selectionLength == 0)
            {
                var deletePosition = textEditor.SelectionStart - 1;
                DeleteFromPosition(deletePosition);
                return;
            }

            ClearSelectedText();
        }

        public void Initialize(string text)
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            _initializing = true;

            try
            {
                var document = textEditor.Document;
                document.Changed -= OnTextDocumentChanged;

                _editingState = EditingState.None;
                text = AdjustText(text);
                UpdateText(text);
                _editingState = EditingState.Editing;

                document.UndoStack.ClearAll();
                document.Changed += OnTextDocumentChanged;

                RefreshHighlightings();
            }
            finally
            {
                _initializing = false;
            }
        }

        public void RefreshView()
        {
            var textEditor = _textEditor;
            var elementGenerator = _elementGenerator;
            if (textEditor is null || elementGenerator is null)
            {
                return;
            }

            elementGenerator.Refresh(textEditor.Text);
            textEditor.TextArea.TextView.Redraw();
        }

        public void GotoPosition(int lineIndex, int columnIndex)
        {
            var textEditor = _textEditor;
            var elementGenerator = _elementGenerator;
            if (textEditor is null || elementGenerator is null)
            {
                return;
            }

            textEditor.SetCaretToSpecificLineAndColumn(lineIndex, columnIndex, elementGenerator.Lines);
        }

        public void GotoPosition(int offset)
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            textEditor.CaretOffset = offset;
        }

        public string GetSelectedText()
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return string.Empty;
            }

            return textEditor.TextArea.Selection.GetText();
        }

        public void SetSelectedText(string text)
        {
            var textEditor = _textEditor;
            var highlightAllOccurrencesOfSelectedWordTransformer = _highlightAllOccurrencesOfSelectedWordTransformer;
            if (textEditor is null || highlightAllOccurrencesOfSelectedWordTransformer is null)
            {
                return;
            }

            highlightAllOccurrencesOfSelectedWordTransformer.SelectedWord = text;

            textEditor.TextArea.TextView.Redraw();
        }

        public void SetSelection(int start, int length)
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            textEditor.SelectionStart = start;
            textEditor.SelectionLength = length;

            if (length == 0)
            {
                return;
            }

            textEditor.TextArea.TextView.Redraw();
        }

        public bool IsCaretWithinQuotedField()
        {
            var textEditor = _textEditor;
            var elementGenerator = _elementGenerator;
            if (textEditor is null || elementGenerator is null)
            {
                return false;
            }

            var caretPosition = textEditor.CaretOffset;
            var textDocument = textEditor.Document;
            var caretLocation = textDocument.GetLocation(caretPosition);
            var column = elementGenerator.GetColumn(caretLocation);
            if (column is null)
            {
                return false;
            }

            var text = textDocument.Text;
            var currentLine = textDocument.GetLineByNumber(caretLocation.Line);

            var insertPosition = currentLine.Offset + column.Offset;

            return text.Length > insertPosition && text[insertPosition] == Symbols.Quote;
        }

        public void InsertAtCaret(char character)
        {
            var textEditor = _textEditor;
            if (textEditor is null)
            {
                return;
            }

            textEditor.Document.Insert(textEditor.CaretOffset, character.ToString());
        }

        public event EventHandler<CaretTextLocationChangedEventArgs>? CaretTextLocationChanged;
        public event EventHandler<EventArgs>? TextChanged;
        public event EventHandler<EventArgs>? EditorAttached;
        public event EventHandler<EventArgs>? EditorDetached;
    }
}
