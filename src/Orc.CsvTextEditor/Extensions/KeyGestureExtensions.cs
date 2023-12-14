namespace Orc.CsvTextEditor
{
    using System;
    using System.Windows.Input;

    public static class KeyGestureExtensions
    {
        public static bool IsKeyAndModifierEquals(this KeyGesture left, KeyGesture right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            return left.Key == right.Key && left.Modifiers == right.Modifiers;
        }
    }
}
