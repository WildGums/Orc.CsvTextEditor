// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextSynchronizationScope.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using Catel;

    public class CsvTextSynchronizationScope : Disposable
    {
        #region Fields
        private readonly CsvTextSynchronizationService _csvTextSynchronizationService;
        #endregion

        #region Constructors
        public CsvTextSynchronizationScope(CsvTextSynchronizationService csvTextSynchronizationService)
        {
            Argument.IsNotNull(() => csvTextSynchronizationService);

            _csvTextSynchronizationService = csvTextSynchronizationService;
            _csvTextSynchronizationService.IsSynchronizing = true;
        }
        #endregion

        #region Methods
        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            _csvTextSynchronizationService.IsSynchronizing = false;
        }
        #endregion
    }
}
