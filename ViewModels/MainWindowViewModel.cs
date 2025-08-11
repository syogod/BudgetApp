using System.Collections.ObjectModel;
using BudgetApp.Models;
using BudgetApp.Data;

namespace BudgetApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<CategoryModel> Categories { get; set; }

    public MainWindowViewModel()
    {
        using (var context = new ApplicationDbContext())
        {
            Categories = new ObservableCollection<CategoryModel>(context.Categories);
        }
    }
}