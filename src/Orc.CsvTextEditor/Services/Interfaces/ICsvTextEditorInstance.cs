﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICsvTextEditorInstance.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;
    using System.Collections.Generic;
    using Controls;
    using Controls.Tools;
    using Operations;

    public interface ICsvTextEditorInstance : IDisposable
    {
        #region Properties
        IEnumerable<IControlTool> Tools { get; }
        IControlToolManager ToolManager { get; }
        int LinesCount { get; }
        int ColumnsCount { get; }
        bool IsAutocompleteEnabled { get; set; }
        bool HasSelection { get; }
        bool CanRedo { get; }
        bool CanUndo { get; }
        string LineEnding { get; }
        bool IsDirty { get; }
        #endregion

        #region Methods
        void AttachEditor(object editor);
        void DetachEditor();
        object GetEditor();

        bool IsCaretWithinQuotedField();

        void InsertAtCaret(char character);

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

        string GetSelectedText();
        void SetSelectedText(string text);
        void SetSelection(int start, int length);
        string GetText();
        void SetText(string text);
        void SetInitialText(string text);
        #endregion

        #region Events
        event EventHandler<CaretTextLocationChangedEventArgs> CaretTextLocationChanged;
        event EventHandler<EventArgs> TextChanged;
        event EventHandler<EventArgs> EditorAttached;
        event EventHandler<EventArgs> EditorDetached;
        #endregion
    }
}
