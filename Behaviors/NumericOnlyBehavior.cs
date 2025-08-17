// File: Behaviors/NumericOnlyBehavior.cs
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace BudgetApp.Behaviors
{
    public class NumericOnlyBehavior : Behavior<TextBox>
    {
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

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            // Allow control keys, digits, and numpad digits
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9) &&
                !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) &&
                e.Key != Key.Back && e.Key != Key.Delete &&
                e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Tab)
            {
                e.Handled = true;
            }
        }
    }
}