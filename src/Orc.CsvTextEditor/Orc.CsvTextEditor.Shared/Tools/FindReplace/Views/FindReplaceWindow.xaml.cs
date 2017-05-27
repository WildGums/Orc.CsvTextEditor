// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindReplaceWindow.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using Catel.MVVM.Views;
    using Catel.Windows;

    internal partial class FindReplaceWindow
    {
        #region Constructors
        static FindReplaceWindow()
        {
            typeof(FindReplaceWindow).AutoDetectViewPropertiesToSubscribe();
        }

        public FindReplaceWindow()
            : base(DataWindowMode.Custom)
        {
            InitializeComponent();
        }
        #endregion
    }
}