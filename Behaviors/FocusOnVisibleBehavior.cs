using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace BudgetApp.Behaviors
{
    public class FocusOnVisibleBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.PropertyChanged += OnPropertyChanged;
                AssociatedObject.AttachedToVisualTree += OnAttachedToVisualTree;
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PropertyChanged -= OnPropertyChanged;
                AssociatedObject.AttachedToVisualTree -= OnAttachedToVisualTree;
            }
            base.OnDetaching();
        }

        private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            if (AssociatedObject?.IsVisible == true)
            {
                Dispatcher.UIThread.Post(() => AssociatedObject.Focus(), DispatcherPriority.Background);
            }
        }
        private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == Avalonia.Controls.Control.IsVisibleProperty && AssociatedObject?.IsVisible == true)
            {
                Dispatcher.UIThread.Post(() => AssociatedObject.Focus(), DispatcherPriority.Background);
            }
        }
    }
}