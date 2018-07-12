// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisableTextDragDropBehavior.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System.Windows;
    using Catel.Windows.Interactivity;
    using ICSharpCode.AvalonEdit;

    internal class DisableTextDragDropBehavior : BehaviorBase<TextEditor>
    {
        private bool _originalAllowDrop;

        #region Methods
        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            _originalAllowDrop = (bool)AssociatedObject.TextArea.GetValue(UIElement.AllowDropProperty);
            AssociatedObject.TextArea.SetCurrentValue(UIElement.AllowDropProperty, true);

            AssociatedObject.TextArea.PreviewDragEnter += OnPreviewDragEnter;
            AssociatedObject.TextArea.PreviewDragOver += OnPreviewDragEnter;
            AssociatedObject.TextArea.PreviewDrop += OnPreviewDrop;
        }

        protected override void OnAssociatedObjectUnloaded()
        {
            AssociatedObject.TextArea.SetCurrentValue(UIElement.AllowDropProperty, _originalAllowDrop);

            AssociatedObject.TextArea.PreviewDragEnter -= OnPreviewDragEnter;
            AssociatedObject.TextArea.PreviewDragOver -= OnPreviewDragEnter;
            AssociatedObject.TextArea.PreviewDrop -= OnPreviewDrop;
        }

        private static void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void OnPreviewDrop(object sender, DragEventArgs e)
        {
            e.Handled = !e.Data.GetDataPresent(DataFormats.FileDrop, true);
        }
        #endregion
    }
}