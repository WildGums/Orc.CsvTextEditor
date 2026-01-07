namespace Orc.CsvTextEditor
{
    using System;
    using System.Collections.Generic;

    public interface ICsvTextEditorInstanceManager
    {
        event EventHandler<CsvTextEditorEventArgs> InstanceRegistered;
        event EventHandler<CsvTextEditorEventArgs> InstanceUnregistered;

        IReadOnlyList<ICsvTextEditorInstance> GetInstances();
        ICsvTextEditorInstance? GetInstance(string id);
        void RegisterInstance(string id, ICsvTextEditorInstance instance);
        bool UnregisterInstance(string id);
    }
}
