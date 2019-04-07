// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReplaceCommandBindingBehavior.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System.Windows;
    using System.Windows.Input;
    using Catel.Windows.Interactivity;
    using ICSharpCode.AvalonEdit;

    internal class ReplaceCommandBindingBehavior : BehaviorBase<TextEditor>
    {
        #region Fields
        private CommandBinding _replacedCommandBinding;
        #endregion

        #region Dependency properties
        public RoutedCommand ReplacementCommand
        {
            get => (RoutedCommand)GetValue(ReplacementCommandProperty);
            set => SetValue(ReplacementCommandProperty, value);
        }

        public static readonly DependencyProperty ReplacementCommandProperty = DependencyProperty.Register(nameof(ReplacementCommand), typeof(RoutedCommand),
            typeof(ReplaceCommandBindingBehavior), new PropertyMetadata(default(RoutedCommand)));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand),
            typeof(ReplaceCommandBindingBehavior), new PropertyMetadata(default(ICommand), (o, args) => ((ReplaceCommandBindingBehavior)o).OnCommandPropertyChanged()));
        #endregion

        #region Methods
        private void OnCommandPropertyChanged()
        {
            var textArea = AssociatedObject?.TextArea;
            if (textArea == null)
            {
                return;
            }

            var commandBindings = textArea.CommandBindings;

            if (_replacedCommandBinding != null)
            {
                commandBindings.Add(_replacedCommandBinding);
            }

            for (var i = 0; i < commandBindings.Count; i++)
            {
                var commandBinding = commandBindings[i];
                if (commandBinding.Command != ReplacementCommand)
                {
                    continue;
                }

                textArea.CommandBindings.Remove(commandBinding);
                textArea.CommandBindings.Add(new CommandBinding(ReplacementCommand, (sender, e) => Command?.Execute(null)));

                _replacedCommandBinding = commandBinding;
                return;
            }

            _replacedCommandBinding = null;
        }
        #endregion
    }
}
