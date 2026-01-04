namespace Orc.CsvTextEditor
{
    using System;

    public interface ICsvTextSynchronizationService
    {
        bool IsSynchronizing { get; }

        IDisposable SynchronizeInScope();
    }
}
