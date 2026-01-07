namespace Orc.CsvTextEditor
{
    using System;

    public class CsvTextEditorEventArgs : EventArgs
    {
        public CsvTextEditorEventArgs(ICsvTextEditorInstance instance)
        {
            Instance = instance;
        }

        public ICsvTextEditorInstance Instance { get; }
    }
}
