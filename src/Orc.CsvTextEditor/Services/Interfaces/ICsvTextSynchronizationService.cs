// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICsvTextSynchronizationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;

    // TODO: replace scope management
    internal interface ICsvTextSynchronizationService
    {
        #region Properties
        bool IsSynchronizing { get; }
        IDisposable SynchronizeInScope();
        #endregion
    }
}