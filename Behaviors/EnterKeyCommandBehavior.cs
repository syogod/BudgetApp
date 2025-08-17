// File: Behaviors/EnterKeyCommandBehavior.cs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using System.Windows.Input;

namespace BudgetApp.Behaviors
{
    public class EnterKeyCommandBehavior : Behavior<TextBox>
    {
        public static readonly StyledProperty<ICommand> CommandProperty =
            AvaloniaProperty.Register<EnterKeyCommandBehavior, ICommand>(nameof(Command));

        public static readonly StyledProperty<object?> CommandParameterProperty =
            AvaloniaProperty.Register<EnterKeyCommandBehavior, object?>(nameof(CommandParameter));

        public ICommand? Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.KeyUp += OnKeyUp;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
                AssociatedObject.KeyUp -= OnKeyUp;
            base.OnDetaching();
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Command?.CanExecute(CommandParameter) == true)
            {
                Command.Execute(CommandParameter);
                e.Handled = true;
            }
        }
    }
}