// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvTextEditorControl.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;
    using System.Windows;
    using Catel.MVVM.Views;
    using Controls;

    public partial class CsvTextEditorControl
    {
        #region Constructors
        static CsvTextEditorControl()
        {
            typeof(CsvTextEditorControl).AutoDetectViewPropertiesToSubscribe();
        }

        public CsvTextEditorControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Dependency properties
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayViewWins)]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(CsvTextEditorControl), new PropertyMetadata(default(string)));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public object Scope
        {
            get { return GetValue(ScopeProperty); }
            set { SetValue(ScopeProperty, value); }
        }

        public static readonly DependencyProperty ScopeProperty = DependencyProperty.Register(nameof(Scope), typeof(object), typeof(CsvTextEditorControl),
            new PropertyMetadata(default(object)));
        #endregion
    }
}
