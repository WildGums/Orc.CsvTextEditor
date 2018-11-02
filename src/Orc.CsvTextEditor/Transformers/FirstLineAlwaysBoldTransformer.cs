// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirstLineAlwaysBoldTransformer.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System.Windows;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Rendering;

    public class FirstLineAlwaysBoldTransformer : DocumentColorizingTransformer
    {
        #region Methods
        protected override void ColorizeLine(DocumentLine line)
        {
            if (line.LineNumber == 1)
            {
                ChangeLinePart(
                    line.Offset, // startOffset
                    line.EndOffset, // endOffset
                    element =>
                    {
                        var tf = element.TextRunProperties.Typeface;
                        element.TextRunProperties.SetTypeface(new Typeface(tf.FontFamily, FontStyles.Normal, FontWeights.ExtraBold, tf.Stretch));
                    });
            }
        }
        #endregion
    }
}
