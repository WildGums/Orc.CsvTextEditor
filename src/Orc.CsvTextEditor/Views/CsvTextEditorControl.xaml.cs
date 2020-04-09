// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextEditorControl.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM.Views;
    using Orc.Controls;

    public partial class CsvTextEditorControl
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ITypeFactory _typeFactory;
        private readonly IServiceLocator _serviceLocator;
        private ICsvTextSynchronizationService _synchronizationService;

        #region Constructors
        static CsvTextEditorControl()
        {
            typeof(CsvTextEditorControl).AutoDetectViewPropertiesToSubscribe();
        }

        public CsvTextEditorControl()
        {
            _serviceLocator = this.GetServiceLocator();
            _typeFactory = _serviceLocator.ResolveType<ITypeFactory>();

            InitializeComponent();
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Customize Type of wrapper used for underlying TextEditor
        /// </summary>
        public Type EditorInstanceType
        {
            get { return (Type)GetValue(EditorInstanceTypeProperty); }
            set { SetValue(EditorInstanceTypeProperty, value); }
        }

        public static readonly DependencyProperty EditorInstanceTypeProperty = DependencyProperty.Register("EditorInstanceType", typeof(Type), typeof(CsvTextEditorControl),
                new PropertyMetadata(typeof(CsvTextEditorInstance), (sender, e) => (sender as CsvTextEditorControl).OnTextEditorWrapperChanged(e)));

        private void OnTextEditorWrapperChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateServiceRegistration();
        }

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayViewWins)]
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(CsvTextEditorControl), new PropertyMetadata(default(string)));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public ICsvTextEditorInstance CsvTextEditorInstance
        {
            get { return (ICsvTextEditorInstance)GetValue(CsvTextEditorInstanceProperty); }
            set { SetValue(CsvTextEditorInstanceProperty, value); }
        }

        public static readonly DependencyProperty CsvTextEditorInstanceProperty =
            DependencyProperty.Register("CsvTextEditorInstance", typeof(ICsvTextEditorInstance), typeof(CsvTextEditorControl), new PropertyMetadata());

        #endregion

        protected override void OnInitialized(EventArgs e)
        {
            TextEditor.TextChanged += OnTextEditorTextChanged;
            base.OnInitialized(e);
        }

        private void OnTextEditorTextChanged(object sender, EventArgs e)
        {
            if (_synchronizationService?.IsSynchronizing ?? true)
            {
                return;
            }

            var textEditor = TextEditor;
            if (textEditor == null)
            {
                return;
            }

            using (_synchronizationService.SynchronizeInScope())
            {
                SetCurrentValue(TextProperty, textEditor.Text);
            }
        }

        protected override void OnViewModelChanged()
        {
            _synchronizationService = _serviceLocator.ResolveType<ICsvTextSynchronizationService>(ViewModel);
            UpdateServiceRegistration();
            base.OnViewModelChanged();
        }

        private void UpdateServiceRegistration()
        {
            var textEditorControl = this;
            var textEditor = textEditorControl.TextEditor;
            var wrapperInstanceType = textEditorControl.EditorInstanceType;
            if (textEditor == null || wrapperInstanceType == null)
            {
                return;
            }

            if (!typeof(ICsvTextEditorInstance).IsAssignableFrom(wrapperInstanceType))
            {
                Log.Error($"Cannot use type {wrapperInstanceType} because it not implemented ICsvTextEditorInstance");
            }

            var csvTextEditorInstance = (ICsvTextEditorInstance)_typeFactory.CreateInstanceWithParametersAndAutoCompletion(wrapperInstanceType, textEditor);

            SetCurrentValue(CsvTextEditorInstanceProperty, csvTextEditorInstance);

            textEditorControl.AttachTool<FindReplaceTool>();
        }
    }
}
