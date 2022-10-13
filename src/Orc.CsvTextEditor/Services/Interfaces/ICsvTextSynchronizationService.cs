namespace Orc.CsvTextEditor
{
    using System;

    internal interface ICsvTextSynchronizationService
    {
        bool IsSynchronizing { get; }

        IDisposable SynchronizeInScope();
    }
}
