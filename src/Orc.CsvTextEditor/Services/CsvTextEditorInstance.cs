// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextEditorInstance.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Controls;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.CodeCompletion;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using Operations;

    public class CsvTextEditorInstance : Disposable, ICsvTextEditorInstance
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ICommandManager _commandManager;
        private readonly ICsvTextEditorInitializer _initializer;
        private readonly IDispatcherService _dispatcherService;
        private readonly DispatcherTimer _refreshViewTimer;
        private readonly List<IControlTool> _tools;
        private readonly ITypeFactory _typeFactory;

        private TextEditor _textEditor;
        private TabSpaceElementGenerator _elementGenerator;
        private HighlightAllOccurencesOfSelectedWordTransformer _highlightAllOccurrencesOfSelectedWordTransformer;
        private CompletionWindow _completionWindow;
        private EditingState _editingState = EditingState.None;
        private bool _initializing;
        private bool _isInCustomUpdate;
        private bool _isInRedoUndo;
        private Location _lastLocation;
        private int _textChangingIterator;
        #endregion

        #region Constructors
        public CsvTextEditorInstance(TextEditor textEditor, ICommandManager commandManager, ICsvTextEditorInitializer initializer,
            IDispatcherService dispatcherService, ITypeFactory typeFactory)
            : this(commandManager, initializer, dispatcherService, typeFactory)
        {
            Argument.IsNotNull(() => textEditor);

            AttachEditor(textEditor);
        }

        public CsvTextEditorInstance(ICommandManager commandManager, ICsvTextEditorInitializer initializer,
            IDispatcherService dispatcherService, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => commandManager);
            Argument.IsNotNull(() => initializer);
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => typeFactory);

            _commandManager = commandManager;
            _initializer = initializer;
            _dispatcherService = dispatcherService;
            _typeFactory = typeFactory;

            _tools = new List<IControlTool>();

            _refreshViewTimer = new DispatcherTimer();
            _refreshViewTimer.Tick += (sender, e) =>
            {
                RefreshView();
                ((DispatcherTimer)sender).Stop();
            };
            _refreshViewTimer.Interval = TimeSpan.FromMilliseconds(50);
        }
        #endregion

        #region Properties
        public IEnumerable<IControlTool> Tools => _tools;
        public int LinesCount => _textEditor?.Document?.LineCount ?? 0;
        public int ColumnsCount => _elementGenerator.ColumnCount;
        public bool IsAutocompleteEnabled { get; set; } = true;
        public bool HasSelection => _textEditor.SelectionLength > 0;
        public bool CanRedo => !_initializing && _textEditor.CanRedo;
        public bool CanUndo => !_initializing && _textEditor.CanUndo;
        public string LineEnding => _elementGenerator.NewLine;
        public bool IsDirty => _textChangingIterator != 0;
        public void AttachEditor(object editor)
        {
            Argument.IsNotNull(() => editor);

            DetachEditor();

            _textEditor = editor as TextEditor;
            if (_textEditor is null)
            {
                return;
            }

            // Need to make these options accessible to the user in the settings window
            _textEditor.SetCurrentValue(TextEditor.ShowLineNumbersProperty, true);
            _textEditor.Options.HighlightCurrentLine = true;
            _textEditor.Options.ShowEndOfLine = true;
            _textEditor.Options.ShowTabs = true;

            _elementGenerator = _typeFactory.CreateInstance<TabSpaceElementGenerator>();

            _textEditor.TextArea.TextView.ElementGenerators.Add(_elementGenerator);

            _textEditor.TextArea.SelectionChanged += OnTextAreaSelectionChanged;
            _textEditor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
            _textEditor.TextArea.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            _textEditor.TextChanged += OnTextChanged;
            _textEditor.PreviewKeyDown += OnPreviewKeyDown;

            _textEditor.TextArea.TextEntering += OnTextEntering;

            _highlightAllOccurrencesOfSelectedWordTransformer = new HighlightAllOccurencesOfSelectedWordTransformer();
            _textEditor.TextArea.TextView.LineTransformers.Add(_highlightAllOccurrencesOfSelectedWordTransformer);
            _textEditor.TextArea.TextView.LineTransformers.Add(new FirstLineAlwaysBoldTransformer());

            _initializer.Initialize(_textEditor, this);
        }

        public void DetachEditor()
        {
            if (_textEditor is null)
            {
                return;
            }

            _textEditor.TextArea.SelectionChanged -= OnTextAreaSelectionChanged;
            _textEditor.TextArea.Caret.PositionChanged -= OnCaretPositionChanged;
            _textEditor.TextArea.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            _textEditor.TextChanged -= OnTextChanged;
            _textEditor.PreviewKeyDown -= OnPreviewKeyDown;
            _textEditor.TextArea.TextEntering -= OnTextEntering;

            _elementGenerator = null;
            _highlightAllOccurrencesOfSelectedWordTransformer = null;
        }
        #endregion

        #region ICsvTextEditorInstance Members
        public object GetEditor()
        {
            return _textEditor;
        }

        public string GetText()
        {
            var text = string.Empty;

            _dispatcherService.Invoke(() => text = _textEditor.Text);

            return text;
        }

        public void SetText(string text)
        {
            _dispatcherService.Invoke(() => UpdateText(text));
        }

        public void AddTool(IControlTool tool)
        {
            Argument.IsNotNull(() => tool);

            if (_tools.Contains(tool))
            {
                return;
            }

            _tools.Add(tool);
        }

        public void RemoveTool(IControlTool tool)
        {
            Argument.IsNotNull(() => tool);

            _tools.Remove(tool);
            tool.Close();
        }

        public void ResetIsDirty()
        {
            _textChangingIterator = 0;
        }

        public void Copy()
        {
            _textEditor.Copy();
        }

        public void Paste()
        {
            var text = Clipboard.GetText();
            text = text.Replace(Symbols.Comma.ToString(), string.Empty)
                .Replace(_elementGenerator.NewLine, string.Empty);
            _textEditor.Document.Replace(_textEditor.SelectionStart, _textEditor.SelectionLength, text);
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
            var selectedText = _textEditor.SelectedText;

            ClearSelectedText();

            Clipboard.SetText(selectedText);
        }

        public Location GetLocation()
        {
            var textDocument = _textEditor.Document;
            var offset = _textEditor.CaretOffset;
            var textLocation = textDocument.GetLocation(offset);

            var column = _elementGenerator.GetColumn(textLocation);
            if (column == null)
            {
                return null;
            }

            var lineIndex = textLocation.Line - 1;
            var documentLine = textDocument.Lines[lineIndex];

            var location = new Location
            {
                Column = column,
                Line = new Line
                {
                    Index = lineIndex,
                    Offset = documentLine.Offset,
                    Length = documentLine.Length
                },

                Offset = offset
            };

            return location;
        }

        public void ExecuteOperation<TOperation>() where TOperation : IOperation
        {
            var operation = (TOperation)_typeFactory.CreateInstanceWithParametersAndAutoCompletion(typeof(TOperation), this);
            operation.Execute();
        }

        public void DeleteNextSelectedText()
        {
            var selectionLength = _textEditor.SelectionLength;
            if (selectionLength == 0)
            {
                var deletePosition = _textEditor.SelectionStart;
                DeleteFromPosition(deletePosition);
                return;
            }

            ClearSelectedText();
        }

        public void DeletePreviousSelectedText()
        {
            var selectionLength = _textEditor.SelectionLength;
            if (selectionLength == 0)
            {
                var deletePosition = _textEditor.SelectionStart - 1;
                DeleteFromPosition(deletePosition);
                return;
            }

            ClearSelectedText();
        }

        public void Initialize(string text)
        {
            _initializing = true;

            try
            {
                var document = _textEditor.Document;
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
            _elementGenerator.Refresh(_textEditor.Text);
            _textEditor.TextArea.TextView.Redraw();
        }

        public void GotoPosition(int lineIndex, int columnIndex)
        {
            _textEditor.SetCaretToSpecificLineAndColumn(lineIndex, columnIndex, _elementGenerator.Lines);
        }

        public string GetSelectedText()
        {
            return _textEditor.TextArea.Selection.GetText();
        }

        public bool IsCaretWithinQuotedField()
        {
            var caretPosition = _textEditor.CaretOffset;
            var textDocument = _textEditor.Document;
            var caretLocation = textDocument.GetLocation(caretPosition);
            var column = _elementGenerator.GetColumn(caretLocation);
            var text = textDocument.Text;
            var currentLine = textDocument.GetLineByNumber(caretLocation.Line);

            return text[currentLine.Offset + column.Offset] == Symbols.Quote;
        }

        public void InsertAtCaret(char character)
        {
            _textEditor.Document.Insert(_textEditor.CaretOffset, character.ToString());
        }
        #endregion

        #region Methods
        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            _textEditor.TextArea.SelectionChanged -= OnTextAreaSelectionChanged;
            _textEditor.TextArea.Caret.PositionChanged -= OnCaretPositionChanged;
            _textEditor.TextArea.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            _textEditor.TextChanged -= OnTextChanged;
            _textEditor.PreviewKeyDown -= OnPreviewKeyDown;

            _textEditor.TextArea.TextEntering -= OnTextEntering;

            _textEditor.TextArea.TextView.ElementGenerators.Clear();
            _textEditor.TextArea.TextView.LineTransformers.Clear();

            _refreshViewTimer.Stop();
        }

        private void TextEditorRedo()
        {
            _editingState = EditingState.Redoing;

            try
            {
                _textEditor.Redo();
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
                _textEditor.Undo();
            }
            finally
            {
                _editingState = EditingState.Editing;
            }
        }

        private void OnTextEntering(object sender, TextCompositionEventArgs e)
        {
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

            if (_completionWindow != null)
            {
                return;
            }

            var columnIndex = GetCurrentColumnIndex();
            var data = _textEditor.GetCompletionDataForText(inputText, columnIndex, _elementGenerator.Lines);

            if (!data.Any())
            {
                return;
            }

            _completionWindow = new CompletionWindow(_textEditor.TextArea);
            _completionWindow.CompletionList.CompletionData.AddRange(data);
            _completionWindow.Show();
            _completionWindow.Closed += (o, args) => _completionWindow = null;
        }

        private int GetCurrentColumnIndex()
        {
            var textDocument = _textEditor.Document;
            var offset = _textEditor.CaretOffset;
            var affectedLocation = textDocument.GetLocation(offset);
            var columnNumberWithOffset = _elementGenerator.GetColumn(affectedLocation);
            return columnNumberWithOffset.Index;
        }

        private void RefreshHighlightings()
        {
            using (var customHighlightings = GetType().Assembly.GetManifestResourceStream("Orc.CsvTextEditor.Resources.Highlightings.CustomHighlighting.xshd"))
            {
                if (customHighlightings == null)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Could not find embedded resource");
                }

                using (var reader = new XmlTextReader(customHighlightings))
                {
                    _textEditor.SetCurrentValue(TextEditor.SyntaxHighlightingProperty, HighlightingLoader.Load(reader, HighlightingManager.Instance));
                }
            }
        }

        private void RefreshLocation(int offset, int length)
        {
            if (_isInCustomUpdate || _isInRedoUndo)
            {
                return;
            }

            var textDocument = _textEditor.Document;
            var affectedLocation = textDocument.GetLocation(offset);

            if (_elementGenerator.RefreshLocation(affectedLocation, length))
            {
                // Note: a redraw could be required here
            }
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (_elementGenerator.UnfreezeColumnResizing())
            {
                RefreshView();
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && _elementGenerator.UnfreezeColumnResizing())
            {
                RefreshView();
            }
        }

        private void OnTextDocumentChanged(object sender, DocumentChangeEventArgs e)
        {
            RefreshLocation(e.Offset, e.InsertionLength - e.RemovalLength);
        }

        private void ClearSelectedText()
        {
            var textDocument = _textEditor.Document;
            var selectionStart = _textEditor.SelectionStart;
            var selectionLength = _textEditor.SelectionLength;

            if (selectionLength == 0)
            {
                return;
            }

            var text = textDocument.Text.Remove(selectionStart, selectionLength);

            text = text.RemoveEmptyLines();

            _textEditor.SelectionLength = 0;
            UpdateText(text);

            _textEditor.CaretOffset = text.Length < selectionStart
                ? text.Length
                : selectionStart;
        }

        private void DeleteFromPosition(int deletePosition)
        {
            var textDocument = _textEditor.Document;

            if (deletePosition < 0 || deletePosition >= textDocument.TextLength)
            {
                return;
            }

            var deletingChar = textDocument.Text[deletePosition];

            var textLocation = textDocument.GetLocation(deletePosition + 1);

            var column = _elementGenerator.GetColumn(textLocation);

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
            _elementGenerator.Refresh(text);

            _isInCustomUpdate = true;

            using (_textEditor.Document.RunUpdate())
            {
                _textEditor.Document.Text = text;
            }

            _isInCustomUpdate = false;
        }

        private static string AdjustText(string text)
        {
            text = text ?? string.Empty;

            var newLineSymbol = text.GetNewLineSymbol();
            return text.TrimEnd(newLineSymbol);
        }

        private void OnTextChanged(object sender, EventArgs eventArgs)
        {
            UpdateTextChangingIterator();

            RaiseTextChanged();

            _refreshViewTimer.Stop();
            _refreshViewTimer.Start();
        }

        private void UpdateTextChangingIterator()
        {
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

            _textEditor.SetCurrentValue(TextEditor.IsModifiedProperty, _textChangingIterator == 0);
        }

        private void RaiseTextChanged()
        {
            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCaretPositionChanged(object sender, EventArgs eventArgs)
        {
            var location = GetLocation();
            if (location == null)
            {
                return;
            }

            if (_lastLocation != null && _lastLocation.Offset == location.Offset)
            {
                return;
            }

            CaretTextLocationChanged?.Invoke(this, new CaretTextLocationChangedEventArgs(location));

            _lastLocation = location;
        }

        private void OnTextAreaSelectionChanged(object sender, EventArgs e)
        {
            _commandManager.InvalidateCommands();

            // Disable this line if the user is using the "Find Replace" dialog box
            _highlightAllOccurrencesOfSelectedWordTransformer.SelectedWord = _textEditor.SelectedText;
            _highlightAllOccurrencesOfSelectedWordTransformer.Selection = _textEditor.TextArea.Selection;

            _textEditor.TextArea.TextView.Redraw();
        }
        #endregion

        #region Nested type: EditingState
        private enum EditingState
        {
            None,
            Undoing,
            Redoing,
            Editing
        }
        #endregion

        #region Events
        public event EventHandler<CaretTextLocationChangedEventArgs> CaretTextLocationChanged;
        public event EventHandler<EventArgs> TextChanged;
        #endregion
    }
}
