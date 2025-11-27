namespace Orc.CsvTextEditor
{
    using System.Windows;
    using System.Windows.Input;
    using Catel.Windows.Interactivity;
    using ICSharpCode.AvalonEdit;

    public class ReplaceCommandBindingBehavior : BehaviorBase<TextEditor>
    {
        private CommandBinding? _replacedCommandBinding;

        public RoutedCommand? ReplacementCommand
        {
            get => (RoutedCommand?)GetValue(ReplacementCommandProperty);
            set => SetValue(ReplacementCommandProperty, value);
        }

        public static readonly DependencyProperty ReplacementCommandProperty = DependencyProperty.Register(nameof(ReplacementCommand), typeof(RoutedCommand),
            typeof(ReplaceCommandBindingBehavior), new PropertyMetadata(default(RoutedCommand)));

        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand),
            typeof(ReplaceCommandBindingBehavior), new PropertyMetadata(default(ICommand), (o, args) => ((ReplaceCommandBindingBehavior)o).OnCommandChanged()));

        private void OnCommandChanged()
        {
            var textArea = AssociatedObject?.TextArea;
            if (textArea is null)
            {
                return;
            }

            var commandBindings = textArea.CommandBindings;

            if (_replacedCommandBinding is not null)
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
    }
}
