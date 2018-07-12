// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICsvTextEditorService.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;
    using System.Collections.Generic;
    using Operations;

    public interface ICsvTextEditorInstance : IDisposable
    {
        #region Properties
        IEnumerable<ICsvTextEditorTool> Tools { get; }
        int LinesCount { get; }
        int ColumnsCount { get; }
        bool IsAutocompleteEnabled { get; set; }
        bool HasSelection { get; }

        bool IsCaretWithinQuotedField();

        bool CanRedo { get; }

        void InsertAtCaret(char character);

        bool CanUndo { get; }
        string LineEnding { get; }
        bool IsDirty { get; }
        #endregion

        #region Events
        event EventHandler<CaretTextLocationChangedEventArgs> CaretTextLocationChanged;
        event EventHandler<EventArgs> TextChanged;
        #endregion

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

        void RefreshView();

        void AddTool(ICsvTextEditorTool tool);
        void RemoveTool(ICsvTextEditorTool tool);
        string GetSelectedText();
        string GetText();
        void SetText(string text);
    }
}
