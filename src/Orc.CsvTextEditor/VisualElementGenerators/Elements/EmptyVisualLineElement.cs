﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyVisualLineElement.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System.Text;
    using System.Windows;
    using System.Windows.Media.TextFormatting;
    using ICSharpCode.AvalonEdit.Rendering;

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
            if (!Equals(TextRunProperties.BackgroundBrush, SystemColors.ControlLightLightBrush))
            {
                TextRunProperties.SetBackgroundBrush(SystemColors.ControlLightLightBrush);
            }

            var spacesBuilder = new StringBuilder();

            var spaces = string.Empty;
            for (var i = 0; i < VisualLength - 1; i++)
            {
                spacesBuilder.Append(Symbols.Space);
            }

            spacesBuilder.Append(Symbols.VerticalBar);

            return new TextCharacters(spacesBuilder.ToString(), TextRunProperties);
        }

        public override bool IsWhitespace(int visualColumn)
        {
            return true;
        }
        #endregion
    }
}
