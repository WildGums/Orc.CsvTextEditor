namespace Orc.CsvTextEditor
{
    using System;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Rendering;

    public class GrayedQuotesDocumentColorizingTransformer : DocumentColorizingTransformer
    {
        private static readonly string QuoteString = Symbols.Quote.ToString();
     
        protected override void ColorizeLine(DocumentLine line)
        {
            ArgumentNullException.ThrowIfNull(line);

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
    }
}
