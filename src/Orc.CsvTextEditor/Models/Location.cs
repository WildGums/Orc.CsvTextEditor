// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Location.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    public class Location
    {
        #region Properties
        public Column Column { get; set; }
        public Line Line { get; set; }
        public int Offset { get; set; }
        #endregion
    }
}
