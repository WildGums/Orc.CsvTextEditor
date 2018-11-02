// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextSynchronizationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;

    public class CsvTextSynchronizationService : ICsvTextSynchronizationService
    {
        #region Properties
        public bool IsSynchronizing { get; set; }
        #endregion

        #region ICsvTextSynchronizationService Members
        public IDisposable SynchronizeInScope()
        {
            return new CsvTextSynchronizationScope(this);
        }
        #endregion
    }
}
