namespace Orc.CsvTextEditor
{
    using System;
    using Catel;

    public class CsvTextSynchronizationScope : Disposable
    {
        private readonly CsvTextSynchronizationService _csvTextSynchronizationService;

        public CsvTextSynchronizationScope(CsvTextSynchronizationService csvTextSynchronizationService)
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
