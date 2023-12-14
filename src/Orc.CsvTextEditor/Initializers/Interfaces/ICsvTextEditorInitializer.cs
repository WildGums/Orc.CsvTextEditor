namespace Orc.CsvTextEditor
{
    using ICSharpCode.AvalonEdit;

    public interface ICsvTextEditorInitializer
    {
        void Initialize(TextEditor textEditor, ICsvTextEditorInstance csvTextEditorInstance);
    }
}
