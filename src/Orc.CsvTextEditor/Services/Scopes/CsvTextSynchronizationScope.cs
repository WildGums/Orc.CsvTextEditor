namespace Orc.CsvTextEditor
{
    using System;
    using Catel;

    public class CsvTextSynchronizationScope : Disposable
    {
        private readonly ICsvTextSynchronizationService _csvTextSynchronizationService;

        public CsvTextSynchronizationScope(ICsvTextSynchronizationService csvTextSynchronizationService)
        {
            ArgumentNullException.ThrowIfNull(csvTextSynchronizationService);

            _csvTextSynchronizationService = csvTextSynchronizationService;
            _csvTextSynchronizationService.IsSynchronizing = true;
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            _csvTextSynchronizationService.IsSynchronizing = false;
        }
    }
}
