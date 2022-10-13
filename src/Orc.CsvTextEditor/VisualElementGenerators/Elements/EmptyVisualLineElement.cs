namespace Orc.CsvTextEditor
{
    using System;
    using System.Text;
    using System.Windows.Media.TextFormatting;
    using ICSharpCode.AvalonEdit.Rendering;
    using Theming;

    internal class EmptyVisualLineElement : VisualLineElement
    {
        public EmptyVisualLineElement(int visualLength, int documentLength)
            : base(visualLength, documentLength)
        {
            
        }

        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

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
    }
}
