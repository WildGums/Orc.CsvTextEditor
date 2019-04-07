// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaretTextLocationChangedEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;

    public class CaretTextLocationChangedEventArgs : EventArgs
    {
        #region Constructors
        public CaretTextLocationChangedEventArgs(Location location)
        {
            Location = location;
        }
        #endregion

        #region Properties
        public Location Location { get; }
        #endregion
    }
}
