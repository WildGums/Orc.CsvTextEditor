// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyVisualLineElement.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System.Text;
    using System.Windows.Media.TextFormatting;
    using ICSharpCode.AvalonEdit.Rendering;
    using Theming;

    internal class EmptyVisualLineElement : VisualLineElement
    {
        #region Constructors
        public EmptyVisualLineElement(int visualLength, int documentLength)
            : base(visualLength, documentLength)
        {
            
        }
        #endregion

        #region Methods
        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            var foreground = ThemeManager.Current.GetThemeColorBrush(ThemeColorStyle.DefaultForeground);
            var background = ThemeManager.Current.GetThemeColorBrush(ThemeColorStyle.DefaultBackground);

            TextRunProperties.SetBackgroundBrush(background);
            TextRunProperties.SetForegroundBrush(foreground);

            var spacesBuilder = new StringBuilder();
            for (var i = 0; i < VisualLength - 1; i++)
            {
                spacesBuilder.Append(Symbols.Space);
            }

            spacesBuilder.Append(Symbols.VerticalBar);

            var textCharacters = new TextCharacters(spacesBuilder.ToString(), TextRunProperties);
            return textCharacters;
        }

        public override bool IsWhitespace(int visualColumn) => true;
        #endregion
    }
}
