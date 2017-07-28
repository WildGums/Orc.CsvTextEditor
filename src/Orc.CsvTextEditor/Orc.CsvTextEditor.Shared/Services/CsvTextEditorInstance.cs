// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextEditorInstance.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Xml;
    using Catel;
    using Catel.Logging;
    using Catel.Collections;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.CodeCompletion;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using Operations;

    internal class CsvTextEditorInstance : ICsvTextEditorInstance
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Fields
        private readonly ICommandManager _commandManager;
        private readonly IDispatcherService _dispatcherService;
        private readonly ITypeFactory _typeFactory;
        private readonly TabSpaceElementGenerator _elementGenerator;
        private readonly HighlightAllOccurencesOfSelectedWordTransformer _highlightAllOccurencesOfSelectedWordTransformer;
        private readonly TextEditor _textEditor;
        private readonly List<ICsvTextEditorTool> _tools;
        private CompletionWindow _completionWindow;
        
        private bool _isInCustomUpdate = false;
        private bool _isInRedoUndo = false;

        private Location _lastLocation;

        private bool _initializing;
        #endregion

        #region Constructors
        public CsvTextEditorInstance(TextEditor textEditor, ICommandManager commandManager, ICsvTextEditorInitializer initializer,
            IDispatcherService dispatcherService, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => textEditor);
            Argument.IsNotNull(() => commandManager);
            Argument.IsNotNull(() => initializer);
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => typeFactory);

            _textEditor = textEditor;
            _commandManager = commandManager;
            _dispatcherService = dispatcherService;
            _typeFactory = typeFactory;

            _tools = new List<ICsvTextEditorTool>();

            // Need to make these options accessible to the user in the settings window
            _textEditor.ShowLineNumbers = true;
            _textEditor.Options.HighlightCurrentLine = true;
            _textEditor.Options.ShowEndOfLine = true;
            _textEditor.Options.ShowTabs = true;

            _elementGenerator = typeFactory.CreateInstance<TabSpaceElementGenerator>();

            _textEditor.TextArea.TextView.ElementGenerators.Add(_elementGenerator);

            _textEditor.TextArea.SelectionChanged += OnTextAreaSelectionChanged;
            _textEditor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
            _textEditor.TextArea.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            _textEditor.TextChanged += OnTextChanged;
            _textEditor.PreviewKeyDown += OnPreviewKeyDown;

            _textEditor.TextArea.TextEntering += OnTextEntering;

            _highlightAllOccurencesOfSelectedWordTransformer = new HighlightAllOccurencesOfSelectedWordTransformer();
            _textEditor.TextArea.TextView.LineTransformers.Add(_highlightAllOccurencesOfSelectedWordTransformer);

            _textEditor.TextArea.TextView.LineTransformers.Add(new FirstLineAlwaysBoldTransformer());

            initializer.Initialize(textEditor, this);
        }

        #endregion

        #region Properties
        public IEnumerable<ICsvTextEditorTool> Tools => _tools;
        public int LinesCount => _textEditor?.Document?.LineCount ?? 0;
        public int ColumnsCount => _elementGenerator.ColumnCount;
        public bool IsAutocompleteEnabled { get; set; } = true;
        public bool HasSelection => _textEditor.SelectionLength > 0;
        public bool CanRedo => !_initializing && _textEditor.CanRedo;
        public bool CanUndo => !_initializing && _textEditor.CanUndo;
        public string LineEnding => _elementGenerator.NewLine;
        #endregion

        #region Events
        public event EventHandler<CaretTextLocationChangedEventArgs> CaretTextLocationChanged;
        public event EventHandler<EventArgs> TextChanged;
        #endregion

        #region Methods
        public string GetText()
        {
            var text = string.Empty;

            _dispatcherService.Invoke(() => text = _textEditor.Text, true);

            return text;
        }

        public void SetText(string text)
        {
            _dispatcherService.Invoke(() => UpdateText(text), true);
        }

        public void AddTool(ICsvTextEditorTool tool)
        {
            Argument.IsNotNull(() => tool);

            if (_tools.Contains(tool))
            {
                return;
            }

            _tools.Add(tool);
        }

        public void RemoveTool(ICsvTextEditorTool tool)
        {
            Argument.IsNotNull(() => tool);

            _tools.Remove(tool);
            tool.Close();
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

            var offset = _textEditor.CaretOffset;
            _textEditor.Document.Insert(offset, text);
        }

        public void Redo()
        {
            using (new DisposableToken<CsvTextEditorInstance>(this, x => x.Instance._isInRedoUndo = true, x =>
            {
                RefreshView();
                x.Instance._isInRedoUndo = false;
            }))
            {
                _textEditor.Redo();
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
                _textEditor.Undo();
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

            var lineIndex = textLocation.Line-1;
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
            var selectionLenght = _textEditor.SelectionLength;
            if (selectionLenght == 0)
            {
                var deletePosition = _textEditor.SelectionStart;
                DeleteFromPosition(deletePosition);
                return;
            }

            ClearSelectedText();
        }

        public void DeletePreviousSelectedText()
        {
            var selectionLenght = _textEditor.SelectionLength;
            if (selectionLenght == 0)
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

                text = AdjustText(text);
                UpdateText(text);

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
            using (var s = GetType().Assembly.GetManifestResourceStream("Orc.CsvTextEditor.Resources.Highlightings.CustomHighlighting.xshd"))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not find embedded resource");
                }

                using (var reader = new XmlTextReader(s))
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
                // FIXME: commented, this dramatically affects performance: 
                //_textEditor.TextArea.TextView.Redraw();
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
            if (e.Key == Key.Tab)
            {
                if (_elementGenerator.UnfreezeColumnResizing())
                {
                    RefreshView();
                }
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
            var selectionLenght = _textEditor.SelectionLength;

            if (selectionLenght == 0)
            {
                return;
            }

            var newLine = _elementGenerator.NewLine;

            var text = textDocument.Text.RemoveCommaSeparatedText(selectionStart, selectionLenght, newLine);

            _textEditor.SelectionLength = 0;

            UpdateText(text);
            _textEditor.CaretOffset = selectionStart;
        }

        private void DeleteFromPosition(int deletePosition)
        {
            var textDocument = _textEditor.Document;

            if (deletePosition < 0 || deletePosition >= textDocument.TextLength)
            {
                return;
            }

            var deletingChar = textDocument.Text[deletePosition];
            if (deletingChar == Symbols.NewLineStart || deletingChar == Symbols.Comma || deletingChar == Symbols.NewLineEnd)
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

        private string AdjustText(string text)
        {
            text = text ?? string.Empty;

            var newLineSymbol = text.GetNewLineSymbol();
            return text.TrimEnd(newLineSymbol);
        }

        private void OnTextChanged(object sender, EventArgs eventArgs)
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

            if (_lastLocation == null || _lastLocation.Offset != location.Offset)
            {
                CaretTextLocationChanged?.Invoke(this, new CaretTextLocationChangedEventArgs(location));

                _lastLocation = location;
            }
        }

        public void GotoPosition(int lineIndex, int columnIndex)
        {
            _textEditor.SetCaretToSpecificLineAndColumn(lineIndex, columnIndex, _elementGenerator.Lines);
        }

        private void OnTextAreaSelectionChanged(object sender, EventArgs e)
        {
            _commandManager.InvalidateCommands();

            // Disable this line if the user is using the "Find Replace" dialog box
            _highlightAllOccurencesOfSelectedWordTransformer.SelectedWord = _textEditor.SelectedText;
            _highlightAllOccurencesOfSelectedWordTransformer.Selection = _textEditor.TextArea.Selection;

            RefreshView();
        }

        public void Dispose()
        {
            _textEditor.TextArea.SelectionChanged -= OnTextAreaSelectionChanged;
            _textEditor.TextArea.Caret.PositionChanged -= OnCaretPositionChanged;
            _textEditor.TextArea.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            _textEditor.TextChanged -= OnTextChanged;
            _textEditor.PreviewKeyDown -= OnPreviewKeyDown;

            _textEditor.TextArea.TextEntering -= OnTextEntering;

            _textEditor.TextArea.TextView.ElementGenerators.Clear();
            _textEditor.TextArea.TextView.LineTransformers.Clear();
        }
        #endregion
    }
}