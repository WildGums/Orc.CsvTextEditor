namespace Orc.CsvTextEditor
{
    using System;

    public interface ICsvTextSynchronizationService
    {
        bool IsSynchronizing { get; set; }

        IDisposable SynchronizeInScope();
    }
}
