namespace Orc.CsvTextEditor
{
    using System;
    using ICSharpCode.AvalonEdit;

    public class CsvTextEditorInitializer : ICsvTextEditorInitializer
    {
        public virtual void Initialize(TextEditor textEditor, ICsvTextEditorInstance csvTextEditorInstance)
        {
            ArgumentNullException.ThrowIfNull(textEditor);
            ArgumentNullException.ThrowIfNull(csvTextEditorInstance);

            //this place are reserved for CsvTextEditor initialization
        }
    }
}
