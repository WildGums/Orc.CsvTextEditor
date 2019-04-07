// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextEditorToolBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;
    using Catel;
    using ICSharpCode.AvalonEdit;

    [ObsoleteEx(TreatAsErrorFromVersion = "3.1.0", RemoveInVersion = "3.2.0", Message = "Use ControlToolBase instead")]
    public abstract class CsvTextEditorToolBase : ICsvTextEditorTool
    {
        #region Constructors
        protected CsvTextEditorToolBase(TextEditor textEditor, ICsvTextEditorInstance csvTextEditorInstance)
        {
            Argument.IsNotNull(() => textEditor);
            Argument.IsNotNull(() => csvTextEditorInstance);

            TextEditor = textEditor;
            CsvTextEditorInstance = csvTextEditorInstance;
        }
        #endregion

        #region Properties
        protected TextEditor TextEditor { get; }
        protected ICsvTextEditorInstance CsvTextEditorInstance { get; }
        public abstract string Name { get; }
        public bool IsOpened { get; private set; }
        #endregion

        #region ICsvTextEditorTool Members
        public virtual void Attach(object target)
        {
            //Do nothing
        }

        public virtual void Detach()
        {
            //Do nothing
        }

        public void Open()
        {
            Open(null);
        }

        public virtual void Open(object parameter)
        {
            if (IsOpened)
            {
                return;
            }

            OnOpen();

            IsOpened = true;
            Opened?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Close()
        {
            IsOpened = false;

            RaiseClosedEvent();
        }

        public event EventHandler<EventArgs> Closed;
        public event EventHandler<EventArgs> Opened;
        #endregion

        #region Methods
        protected abstract void OnOpen();

        private void RaiseClosedEvent()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
