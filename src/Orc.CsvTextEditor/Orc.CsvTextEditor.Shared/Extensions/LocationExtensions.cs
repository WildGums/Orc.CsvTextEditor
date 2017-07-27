// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    public static class LocationExtensions
    {
        #region Methods
        public static int GetOffsetInLine(this Location location)
        {
            return location.Offset - location.Line.Offset;
        }
        #endregion
    }
}