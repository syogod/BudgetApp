using System;
using System.Collections.ObjectModel;
using BudgetApp.Models;
using BudgetApp.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace BudgetApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private DateTime _currentDate = DateTime.Now;

    [ObservableProperty]
    private string _currentMonthYear = DateTime.Now.ToString("MMMM yyyy");

    [ObservableProperty]
    private string _previousMonthName = DateTime.Now.AddMonths(-1).ToString("MMMM");
    
    [ObservableProperty]
    private string _nextMonthName = DateTime.Now.AddMonths(1).ToString("MMMM");
    
    [RelayCommand]
    private void PreviousMonth()
    {
        _currentDate = _currentDate.AddMonths(-1);
        UpdateMonthYear();
    }

    [RelayCommand]
    private void NextMonth()
    {
        _currentDate = _currentDate.AddMonths(1);
        UpdateMonthYear();
    }

    private void UpdateMonthYear()
    {
        CurrentMonthYear = _currentDate.ToString("MMMM yyyy");
        PreviousMonthName = _currentDate.AddMonths(-1).ToString("MMMM");
        NextMonthName = _currentDate.AddMonths(1).ToString("MMMM");
    }
    public ObservableCollection<CategoryModel> Categories { get; set; }

    public MainWindowViewModel()
    {
        UpdateMonthYear();
        using (var context = new ApplicationDbContext())
        {
            Categories = new ObservableCollection<CategoryModel>(context.Categories);
        }
    }
}