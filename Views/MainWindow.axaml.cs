using Avalonia.Controls;
using BudgetApp.ViewModels;

namespace BudgetApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}