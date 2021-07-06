namespace Orc.CsvTextEditor
{
    using System;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Rendering;

    public class GrayedQuotesDocumentColorizingTransformer : DocumentColorizingTransformer
    {
        #region Fields
        private static readonly string QuoteString = Symbols.Quote.ToString();
        #endregion

        #region Methods
        protected override void ColorizeLine(DocumentLine line)
        {
            var lineStartOffset = line.Offset;
            var text = CurrentContext.Document.GetText(line);

            var start = 0;
            var index = 0;
            while ((index = text.IndexOf(QuoteString, start, StringComparison.Ordinal)) >= 0)
            {
                var quoteIndex = lineStartOffset + index;

                ChangeLinePart(quoteIndex, // startOffset
                    quoteIndex + 1, // endOffset
                    element => { element.TextRunProperties.SetForegroundBrush(Brushes.Gray);});

                start = index + 1; // search for next occurrence
            }
        }
        #endregion
    }
}
