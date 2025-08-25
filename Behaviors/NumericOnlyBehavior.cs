// File: Behaviors/NumericOnlyBehavior.cs
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace BudgetApp.Behaviors
{
    public class NumericOnlyBehavior : Behavior<TextBox>
    {
        // This behavior can be attached to a TextBox (or similar control) to restrict input to numeric values only.
        // It intercepts user input and prevents non-numeric characters from being entered.
        // Useful for fields where only numbers are valid, such as amounts, quantities, or IDs.
        //
        // Usage: Attach this behavior in XAML to the desired TextBox.
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.KeyDown += OnKeyDown;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
                AssociatedObject.KeyDown -= OnKeyDown;
            base.OnDetaching();
        }

        private static void OnKeyDown(object? sender, KeyEventArgs e)
        {
            // Allow control keys, digits, numpad digits, and minus sign
            if (e.Key is (< Key.D0 or > Key.D9) and (< Key.NumPad0 or > Key.NumPad9) &&
                e.Key != Key.Back && e.Key != Key.Delete &&
                e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Tab &&
                e.Key != Key.OemMinus && e.Key != Key.Subtract)
            {
                e.Handled = true;
            }
        }
    }
}