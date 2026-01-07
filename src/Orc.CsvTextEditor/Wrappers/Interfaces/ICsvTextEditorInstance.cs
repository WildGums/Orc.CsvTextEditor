namespace Orc.CsvTextEditor
{
    using System;
    using System.Collections.Generic;
    using Controls;
    using Controls.Tools;
    using Operations;

    public interface ICsvTextEditorInstance : IDisposable
    {
        string Id { get; }

        IReadOnlyList<IControlTool> Tools { get; }
        IControlToolManager ToolManager { get; }
        int LinesCount { get; }
        int ColumnsCount { get; }
        bool IsAutocompleteEnabled { get; set; }
        bool HasSelection { get; }
        bool CanRedo { get; }
        bool CanUndo { get; }
        string LineEnding { get; }
        bool IsDirty { get; }
        int SelectionStart { get; }
        int SelectionLength { get; }
        string SelectionText { get; }

        void AttachEditor(object editor);
        void DetachEditor();
        object GetEditor();

        bool IsCaretWithinQuotedField();

        void InsertAtCaret(char character);
        void InsertAtPosition(int offset, string str);

        void ResetIsDirty();
        void Copy();
        void Cut();
        void Paste();
        void Redo();
        void Undo();

        void Initialize(string text);

        void ExecuteOperation<TOperation>() where TOperation : IOperation;
        Location GetLocation();

        void DeleteNextSelectedText();
        void DeletePreviousSelectedText();

        void GotoPosition(int lineIndex, int columnIndex);
        void GotoPosition(int offset);

        void RefreshView();

        string GetSelectedText();
        void SetSelectedText(string text);
        void SetSelection(int start, int length);
        string GetText();
        void SetText(string text);
        void SetInitialText(string text);

        event EventHandler<CaretTextLocationChangedEventArgs>? CaretTextLocationChanged;
        event EventHandler<EventArgs>? TextChanged;
        event EventHandler<EventArgs>? EditorAttached;
        event EventHandler<EventArgs>? EditorDetached;
    }
}
