namespace Orc.CsvTextEditor
{
    using System;

    public class CsvTextSynchronizationService : ICsvTextSynchronizationService
    {
        public bool IsSynchronizing { get; set; }

        public IDisposable SynchronizeInScope()
        {
            return new CsvTextSynchronizationScope(this);
        }
    }
}
