// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReplaceKeyInputBindingBehavior.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Catel.Windows.Interactivity;
    using ICSharpCode.AvalonEdit;
    using Microsoft.Xaml.Behaviors;

    public class ReplaceKeyInputBindingBehavior : BehaviorBase<TextEditor>
    {
        #region Fields
        private InputBinding _removedInputBinding;
        private KeyGesture _removedKeyGesture;
        private RoutedCommand _removedRoutedCommand;
        #endregion

        #region Depenendcy properties
        public KeyGesture Gesture
        {
            get => (KeyGesture)GetValue(GestureProperty);
            set => SetValue(GestureProperty, value);
        }

        public static readonly DependencyProperty GestureProperty = DependencyProperty.Register(nameof(Gesture), typeof(KeyGesture),
            typeof(ReplaceKeyInputBindingBehavior), new PropertyMetadata(default(KeyGesture)));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ReplaceKeyInputBindingBehavior),
            new PropertyMetadata(default(ICommand), (o, args) => ((ReplaceKeyInputBindingBehavior)o).OnCommandPropertyChanged(args)));
        #endregion

        #region Methods
        protected override void OnAssociatedObjectLoaded()
        {
            if (_hasPendingUpdate)
            {
                UpdateInputGesture();
            }

            base.OnAssociatedObjectLoaded();
        }

        private void OnCommandPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateInputGesture();
        }

        private bool _hasPendingUpdate;

        private void UpdateInputGesture()
        {
            _hasPendingUpdate = true;

            var textArea = AssociatedObject?.TextArea;
            if (textArea is null)
            {
                return;
            }

            if (Command is null)
            {
                return;
            }

            if (Gesture is null)
            {
                return;
            }

            var commandBindings = textArea.CommandBindings;
            _removedRoutedCommand?.InputGestures.Add(_removedKeyGesture);

            for (var i = 0; i < commandBindings.Count; i++)
            {
                var commandBinding = commandBindings[i];

                var routedCommand = commandBinding.Command as RoutedCommand;
                var gesture = routedCommand?.InputGestures.OfType<KeyGesture>().FirstOrDefault(x => x.IsKeyAndModifierEquals(Gesture));
                if (gesture == null)
                {
                    continue;
                }

                routedCommand.InputGestures.Remove(gesture);

                _removedKeyGesture = gesture;
                _removedRoutedCommand = routedCommand;
                break;
            }

            var inputBindings = textArea.InputBindings;
            if (_removedInputBinding != null)
            {
                inputBindings.Add(_removedInputBinding);
            }

            for (var i = 0; i < inputBindings.Count; i++)
            {
                var inputBinding = inputBindings[i];
                if (!(inputBinding.Gesture is KeyGesture keyGesture))
                {
                    continue;
                }

                if (!keyGesture.IsKeyAndModifierEquals(Gesture))
                {
                    continue;
                }

                inputBindings.Remove(inputBinding);

                _removedInputBinding = inputBinding;
                break;
            }

            inputBindings.Add(new InputBinding(Command, Gesture));

            _hasPendingUpdate = false;
        }
        #endregion
    }
}
