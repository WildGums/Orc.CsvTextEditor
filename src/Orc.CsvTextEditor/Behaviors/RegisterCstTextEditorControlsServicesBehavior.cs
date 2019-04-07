﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterCstTextEditorControlsServicesBehavior.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System.ComponentModel;
    using Catel.IoC;
    using Catel.Windows.Interactivity;
    using Controls;

    internal class RegisterCstTextEditorControlsServicesBehavior : BehaviorBase<CsvTextEditorControl>
    {
        #region Fields
        private readonly IServiceLocator _serviceLocator;
        private readonly ITypeFactory _typeFactory;

        private object _oldScope;
        #endregion

        #region Constructors
        public RegisterCstTextEditorControlsServicesBehavior()
        {
            _serviceLocator = this.GetServiceLocator();
            _typeFactory = _serviceLocator.ResolveType<ITypeFactory>();
        }
        #endregion

        #region Methods
        protected override void OnAttached()
        {
            base.OnAttached();

            UpdateServiceRegistration();

            var textEditorControl = AssociatedObject;
            textEditorControl.PropertyChanged += OnTextEditorControlPropertyChanged;
        }

        protected override void OnAssociatedObjectUnloaded()
        {
            var textEditorControl = AssociatedObject;
            textEditorControl.PropertyChanged -= OnTextEditorControlPropertyChanged;

            base.OnAssociatedObjectUnloaded();
        }

        private void OnTextEditorControlPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
#pragma warning disable WPF1014
            if (args.HasPropertyChanged(nameof(AssociatedObject.Scope)))
#pragma warning restore WPF1014
            {
                UpdateServiceRegistration();
            }
        }

        private void UpdateServiceRegistration()
        {
            var textEditorControl = AssociatedObject;
            var textEditor = textEditorControl.TextEditor;
            if (textEditor == null)
            {
                return;
            }

            var scope = textEditorControl.Scope;
            if (scope == null)
            {
                RemoveServiceRegistration(_oldScope);
                return;
            }

            if (!_serviceLocator.IsTypeRegistered<ICsvTextEditorInstance>(scope))
            {
                var csvTextEditorService = (ICsvTextEditorInstance)_typeFactory.CreateInstanceWithParametersAndAutoCompletion<CsvTextEditorInstance>(textEditor);
                _serviceLocator.RegisterInstance(csvTextEditorService, scope);
            }

            if (!_serviceLocator.IsTypeRegistered<ICsvTextSynchronizationService>(scope))
            {
                var csvTextSynchronizationService = (ICsvTextSynchronizationService)_typeFactory.CreateInstanceWithParametersAndAutoCompletion<CsvTextSynchronizationService>();
                _serviceLocator.RegisterInstance(csvTextSynchronizationService, scope);
            }

            textEditorControl.AttachTool<FindReplaceTool>();

            _oldScope = scope;
        }

        private void RemoveServiceRegistration(object oldScope)
        {
            if (_serviceLocator.IsTypeRegistered<ICsvTextEditorInstance>(oldScope))
            {
                var csvTextEditorService = _serviceLocator.ResolveType<ICsvTextEditorInstance>(oldScope);
                csvTextEditorService.Dispose();

                _serviceLocator.RemoveType<ICsvTextEditorInstance>(oldScope);
            }

            if (_serviceLocator.IsTypeRegistered<ICsvTextSynchronizationService>(oldScope))
            {
                _serviceLocator.RemoveType<ICsvTextSynchronizationService>(oldScope);
            }
        }
        #endregion
    }
}
