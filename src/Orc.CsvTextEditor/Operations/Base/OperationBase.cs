// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Operations
{
    using Catel;

    public abstract class OperationBase : IOperation
    {
        #region Fields
        protected readonly ICsvTextEditorInstance CsvTextEditorInstance;
        #endregion

        #region Constructors
        protected OperationBase(ICsvTextEditorInstance csvTextEditorInstance)
        {
            Argument.IsNotNull(() => csvTextEditorInstance);

            CsvTextEditorInstance = csvTextEditorInstance;
        }
        #endregion

        #region IOperation Members
        public abstract void Execute();
        #endregion
    }
}
