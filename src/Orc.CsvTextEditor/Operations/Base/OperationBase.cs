namespace Orc.CsvTextEditor.Operations
{
    using System;

    public abstract class OperationBase : IOperation
    {
        protected readonly ICsvTextEditorInstance _csvTextEditorInstance;

        protected OperationBase(ICsvTextEditorInstance csvTextEditorInstance)
        {
            ArgumentNullException.ThrowIfNull(csvTextEditorInstance);

            _csvTextEditorInstance = csvTextEditorInstance;
        }

        public abstract void Execute();
    }
}
