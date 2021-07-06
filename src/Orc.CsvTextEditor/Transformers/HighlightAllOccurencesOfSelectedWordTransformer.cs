// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightAllOccurencesOfSelectedWordTransformer.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Editing;
    using ICSharpCode.AvalonEdit.Rendering;

    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This code originally comes from http://stackoverflow.com/questions/9223674/highlight-all-occurrences-of-selected-word-in-avalonedit.
    /// </remarks>
    public class HighlightAllOccurencesOfSelectedWordTransformer : DocumentColorizingTransformer
    {
        #region Properties
        public string SelectedWord { private get; set; }
        public Selection Selection { private get; set; }
        #endregion

        #region Methods
        protected override void ColorizeLine(DocumentLine line)
        {
            if (string.IsNullOrEmpty(SelectedWord))
            {
                return;
            }

            var lineStartOffset = line.Offset;
            var text = CurrentContext.Document.GetText(line);
            var start = 0;
            int index;

            while ((index = text.IndexOf(SelectedWord, start, StringComparison.Ordinal)) >= 0)
            {
                // Don't highlight the current selection
                if (Selection is not null && Selection.StartPosition.Column == index + 1 && Selection.StartPosition.Line == line.LineNumber)
                {
                    start = Selection.EndPosition.Column;

                    if (start >= text.Length)
                    {
                        break;
                    }

                    continue;
                }

                ChangeLinePart(
                    lineStartOffset + index, // startOffset
                    lineStartOffset + index + SelectedWord.Length, // endOffset
                    element => { element.TextRunProperties.SetBackgroundBrush(Brushes.PaleGreen); });

                start = index + 1; // search for next occurrence
            }
        }
        #endregion
    }
}
