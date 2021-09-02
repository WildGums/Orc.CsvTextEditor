// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    using Catel;

    public abstract class OperationBase : IOperation
    {
        #region Fields
        protected readonly ICsvTextEditorInstance _csvTextEditorInstance;
        #endregion

        #region Constructors
        protected OperationBase(ICsvTextEditorInstance csvTextEditorInstance)
        {
            Argument.IsNotNull(() => csvTextEditorInstance);

            _csvTextEditorInstance = csvTextEditorInstance;
        }
        #endregion

        #region IOperation Members
        public abstract void Execute();
        #endregion
    }
}
