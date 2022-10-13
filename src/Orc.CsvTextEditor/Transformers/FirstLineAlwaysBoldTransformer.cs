namespace Orc.CsvTextEditor
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Rendering;

    public class FirstLineAlwaysBoldTransformer : DocumentColorizingTransformer
    {
        protected override void ColorizeLine(DocumentLine line)
        {
            ArgumentNullException.ThrowIfNull(line);

            if (line.LineNumber != 1)
            {
                return;
            }

            ChangeLinePart(line.Offset, // startOffset
                line.EndOffset, // endOffset
                element =>
                {
                    var tf = element.TextRunProperties.Typeface;
                    element.TextRunProperties.SetTypeface(new Typeface(tf.FontFamily, FontStyles.Normal, FontWeights.ExtraBold, tf.Stretch));
                });
        }
    }
}
