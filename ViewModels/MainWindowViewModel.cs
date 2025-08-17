using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using BudgetApp.Models;
using BudgetApp.Data;
using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;
using System.Linq;


namespace BudgetApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // Properties for current, previous, and next month/year
    private DateTime _currentDate = DateTime.Now;
    [ObservableProperty]
    private string _currentMonthYear = DateTime.Now.ToString("MMMM yyyy");
    [ObservableProperty]
    private string _previousMonthName = DateTime.Now.AddMonths(-1).ToString("MMMM");
    [ObservableProperty]
    private string _nextMonthName = DateTime.Now.AddMonths(1).ToString("MMMM");


    public IEnumerable<CategoryModel> NonBillRootCategories =>
        Categories?.Where(c => c.ParentCategoryId == null && c.Enabled == 1 && c.Name != "Bills");

    public IEnumerable<CategoryModel> BillRootCategories =>
        Categories?.Where(c => c.ParentCategoryId == null && c.Enabled == 1 && c.Name == "Bills");
    
    public IEnumerable<CategoryModel> RootCategories =>
        Categories?.Where(c => c.ParentCategoryId == null && c.Enabled == 1);
    
    // Commands to navigate between months
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
        using (var context = new ApplicationDbContext())
        {
            Transactions = new ObservableCollection<TransactionModel>(context.Transactions);
        }
        CurrentMonthYear = _currentDate.ToString("MMMM yyyy");
        PreviousMonthName = _currentDate.AddMonths(-1).ToString("MMMM");
        NextMonthName = _currentDate.AddMonths(1).ToString("MMMM");

        // Recalculate MonthlySum for each category
        using (var context = new ApplicationDbContext())
        {
            var transactions = context.Transactions
                .Where(t => t.Month == _currentDate.Month && t.Year == _currentDate.Year)
                .ToList();

            foreach (var category in Categories)
            {
                category.MonthlySum = transactions
                    .Where(t => t.CategoryID == category.CategoryId)
                    .Sum(t => t.Amount);
                var lastThreeMonths = Enumerable.Range(1, 3)
                    .Select(i => _currentDate.AddMonths(-i))
                    .Select(d => new { d.Month, d.Year })
                    .ToList();

                category.TriMonthlyAvg = Convert.ToInt32(
                    Transactions
                        .Where(t => t.CategoryID == category.CategoryId &&
                                    lastThreeMonths.Any(m => t.Month == m.Month && t.Year == m.Year))
                        .Select(t => t.Amount)
                        .DefaultIfEmpty(0)
                        .Sum()
                )/3;
            }
            
        }
    }
    
    // Collection of categories
    private ObservableCollection<CategoryModel> Categories { get; set; }
    
    // Collection of transactions
    private ObservableCollection<TransactionModel> Transactions { get; set; }
    
    // Command to add a new transaction
    [RelayCommand]
    private void AddTransaction(CategoryModel category)
    {
        if (category.NewTransactionAmount == 0 || category.NewTransactionAmount == null)
            return;

        using (var context = new ApplicationDbContext())
        {
            var transaction = new TransactionModel
            {
                Month = _currentDate.Month,
                Year = _currentDate.Year,
                CategoryID = category.CategoryId,
                Amount = category.NewTransactionAmount.Value,
            };
            context.Transactions.Add(transaction);
            context.SaveChanges();
        }

        // Optionally update local collection and MonthlySum
        category.MonthlySum += category.NewTransactionAmount.Value;
        category.NewTransactionAmount = null;
        
        // Recalculate TriMonthlyAvg
        var lastThreeMonths = Enumerable.Range(1, 3)
            .Select(i => _currentDate.AddMonths(-i))
            .Select(d => new { d.Month, d.Year })
            .ToList();

        category.TriMonthlyAvg = Convert.ToInt32(
            Transactions
                .Where(t => t.CategoryID == category.CategoryId &&
                            lastThreeMonths.Any(m => t.Month == m.Month && t.Year == m.Year))
                .Select(t => t.Amount)
                .DefaultIfEmpty(0)
                .Sum()
        )/3;
    }
    
    [RelayCommand]
    private void AddCategory(int parentCategoryId)
    {
        var parent = Categories.FirstOrDefault(c => c.CategoryId == parentCategoryId);
        if (parent != null)
        {
            parent.IsAddingSubcategory = !parent.IsAddingSubcategory; // Toggle visibility
            if (!parent.IsAddingSubcategory)
                parent.NewSubcategoryName = string.Empty; // Optionally clear input when hiding
            OnPropertyChanged(nameof(NonBillRootCategories));
            OnPropertyChanged(nameof(BillRootCategories));
        }
    }
    
    [RelayCommand]
    private void SaveSubcategory(CategoryModel parentCategory)
    {
        if (string.IsNullOrWhiteSpace(parentCategory.NewSubcategoryName))
            return;

        var newSubcategory = new CategoryModel
        {
            CategoryId = 0, // New category, ID will be generated by the database
            Name = parentCategory.NewSubcategoryName,
            ParentCategoryId = parentCategory.CategoryId,
            Income = 0, // Default value, can be changed later
            Enabled = 1 // Default enabled state
        };

        using (var context = new ApplicationDbContext())
        {
            context.Categories.Add(newSubcategory);
            context.SaveChanges();
        }

        // Update local collection and reset adding state
        Categories.Add(newSubcategory);
        
        parentCategory.SubCategories = new ObservableCollection<CategoryModel>(Categories.Where(c => c.ParentCategoryId == parentCategory.CategoryId && c.Enabled == 1));
        
        
        parentCategory.IsAddingSubcategory = false;
        parentCategory.NewSubcategoryName = null;
        
        OnPropertyChanged(nameof(NonBillRootCategories));
        OnPropertyChanged(nameof(BillRootCategories));
    }
    public MainWindowViewModel()
    {
        using (var context = new ApplicationDbContext())
        {
            Categories = new ObservableCollection<CategoryModel>(context.Categories);
            Transactions = new ObservableCollection<TransactionModel>(context.Transactions);
        }
        // Initialize month/year display
        UpdateMonthYear();
        
        // Load categories and transactions from the database
        using (var context = new ApplicationDbContext())
        {
            Categories = new ObservableCollection<CategoryModel>(context.Categories);
            Transactions = new ObservableCollection<TransactionModel>(context.Transactions);
        }

        // Build hierarchical category structure
        foreach (var root in Categories)
        {
            root.SubCategories = new ObservableCollection<CategoryModel>(
                Categories.Where(c => c.ParentCategoryId == root.CategoryId && c.Enabled == 1));
        }
        // Calculate monthly sums for each category
        foreach (var category in Categories)
        {
            category.MonthlySum = Transactions
                .Where(t => t.CategoryID == category.CategoryId &&
                            t.Month == _currentDate.Month &&
                            t.Year == _currentDate.Year)
                .Sum(t => t.Amount);
        }

        foreach (var category in Categories)
        {
            var lastThreeMonths = Enumerable.Range(1, 3)
                .Select(i => _currentDate.AddMonths(-i))
                .Select(d => new { d.Month, d.Year })
                .ToList();

            category.TriMonthlyAvg = Convert.ToInt32(
                Transactions
                    .Where(t => t.CategoryID == category.CategoryId &&
                                lastThreeMonths.Any(m => t.Month == m.Month && t.Year == m.Year))
                    .Select(t => t.Amount)
                    .DefaultIfEmpty(0)
                    .Sum()/3
            );
        }
    }
}