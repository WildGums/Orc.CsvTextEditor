namespace Orc.CsvTextEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Collections;
    using Catel.Logging;
    using Microsoft.Extensions.Logging;

    public class CsvTextEditorInstanceManager : ICsvTextEditorInstanceManager
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(CsvTextEditorInstanceManager));

        private readonly Dictionary<string, ICsvTextEditorInstance> _instances = new Dictionary<string, ICsvTextEditorInstance>(StringComparer.OrdinalIgnoreCase);

        public CsvTextEditorInstanceManager()
        {

        }

        public event EventHandler<CsvTextEditorEventArgs>? InstanceRegistered;
        public event EventHandler<CsvTextEditorEventArgs>? InstanceUnregistered;

        public void RegisterInstance(string id, ICsvTextEditorInstance instance)
        {
            if (_instances.ContainsKey(id))
            {
                throw Logger.LogErrorAndCreateException<InvalidOperationException>("An instance is already registered with this ID");
            }

            _instances[id] = instance;

            InstanceRegistered?.Invoke(this, new CsvTextEditorEventArgs(instance));
        }

        public bool UnregisterInstance(string id)
        {
            if (!_instances.TryGetValue(id, out var instance))
            {
                return false;
            }

            _instances.Remove(id);

            InstanceUnregistered?.Invoke(this, new CsvTextEditorEventArgs(instance));

            instance.Dispose();

            return true;
        }

        public ICsvTextEditorInstance? GetInstance(string id)
        {
            if (!_instances.TryGetValue(id, out var instance))
            {
                return null;
            }

            return instance;
        }

        public IReadOnlyList<ICsvTextEditorInstance> GetInstances()
        {
            return _instances.Values.ToArray();
        }
    }
}
