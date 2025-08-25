using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using BudgetApp.Models;
using BudgetApp.Data;
using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;
using System.Linq;
using Avalonia.Controls;


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


    public IEnumerable<CategoryModel>? NonBillRootCategories =>
        Categories?.Where(c => c.ParentCategoryId == null && c is { Enabled: 1, Name: not "Bills" });

    public IEnumerable<CategoryModel>? BillRootCategories =>
        Categories?.Where(c => c.ParentCategoryId == null && c is { Enabled: 1, Name: "Bills" });
    
    // Summary properties
  public int IncomeSum => Categories?
        .Where(c => c.ParentCategoryId == null && c is { Enabled: 1, Income: 1 })
        .Sum(c => c.MonthlyParentCategorySum) ?? 0;

    public int ExpenseSum => Categories?
        .Where(c => c.ParentCategoryId == null && c is { Enabled: 1, Income: 0 })
        .Sum(c => c.MonthlyParentCategorySum) ?? 0;

    public int NetTotal => IncomeSum - ExpenseSum;
    
    public int IncomeEstSum => Categories?
        .Where(c => c.ParentCategoryId == null && c is { Enabled: 1, Income: 1 })
        .Sum(c => c.MonthlyParentCategoryEst) ?? 0;

    public int ExpenseEstSum => Categories?
        .Where(c => c.ParentCategoryId == null && c is { Enabled: 1, Income: 0 })
        .Sum(c => c.MonthlyParentCategoryEst) ?? 0;

    public int NetEstTotal => IncomeEstSum - ExpenseEstSum;
    
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

    // Update month/year display and recalculate MonthlySum for each category
    private void UpdateMonthYear()
    {
        CurrentMonthYear = _currentDate.ToString("MMMM yyyy");
        PreviousMonthName = _currentDate.AddMonths(-1).ToString("MMMM");
        NextMonthName = _currentDate.AddMonths(1).ToString("MMMM");
        
        using (var context = new ApplicationDbContext())
        {
            var transactions = context.Transactions
                .Where(t => t.Month == _currentDate.Month && t.Year == _currentDate.Year)
                .ToList();
            
            foreach (var category in Categories)
            {
                category.MonthlySum = transactions
                    .Where(t => t.CategoryId == category.CategoryId)
                    .Sum(t => t.Amount);
                var lastThreeMonths = Enumerable.Range(1, 3)
                    .Select(i => _currentDate.AddMonths(-i))
                    .Select(d => new { d.Month, d.Year })
                    .ToList();
                
                category.TriMonthlyAvg = Convert.ToInt32(
                    Transactions
                        .Where(t => t.CategoryId == category.CategoryId &&
                                    lastThreeMonths.Any(m => t.Month == m.Month && t.Year == m.Year))
                        .Select(t => t.Amount)
                        .DefaultIfEmpty(0)
                        .Sum()
                )/3;
            }
           
        }
        
        // Notify summary properties changed
        OnPropertyChanged(nameof(IncomeSum));
        OnPropertyChanged(nameof(ExpenseSum));
        OnPropertyChanged(nameof(NetTotal));
        OnPropertyChanged(nameof(IncomeEstSum));
        OnPropertyChanged(nameof(ExpenseEstSum));
        OnPropertyChanged(nameof(NetEstTotal));
    }
    
    private ObservableCollection<CategoryModel> Categories { get; set; }
    private ObservableCollection<TransactionModel> Transactions { get; set; }
    
    // Command to add a new transaction
    [RelayCommand]
    private void AddTransaction(CategoryModel category)
    {
        if (category.NewTransactionAmount is 0 or null)
            return;

        using (var context = new ApplicationDbContext())
        {
            var transaction = new TransactionModel
            {
                Month = _currentDate.Month,
                Year = _currentDate.Year,
                CategoryId = category.CategoryId,
                Amount = category.NewTransactionAmount.Value,
            };
            context.Transactions.Add(transaction);
            context.SaveChanges();
        }

        // Update local Transactions collection
        category.MonthlySum += category.NewTransactionAmount.Value;
        category.NewTransactionAmount = null;
        
        // Recalculate TriMonthlyAvg
        var lastThreeMonths = Enumerable.Range(1, 3)
            .Select(i => _currentDate.AddMonths(-i))
            .Select(d => new { d.Month, d.Year })
            .ToList();

        // Notify summary properties changed
        OnPropertyChanged(nameof(IncomeSum));
        OnPropertyChanged(nameof(ExpenseSum));
        OnPropertyChanged(nameof(NetTotal));
        OnPropertyChanged(nameof(IncomeEstSum));
        OnPropertyChanged(nameof(ExpenseEstSum));
        OnPropertyChanged(nameof(NetEstTotal));

        // Recalculate TriMonthlyAvg
        category.TriMonthlyAvg = Convert.ToInt32(
            Transactions
                .Where(t => t.CategoryId == category.CategoryId &&
                            lastThreeMonths.Any(m => t.Month == m.Month && t.Year == m.Year))
                .Select(t => t.Amount)
                .DefaultIfEmpty(0)
                .Sum()
        )/3;
    }
    
    // Command to toggle adding a new subcategory
    [RelayCommand]
    private void AddCategory(int parentCategoryId)
    {
        var parent = Categories.FirstOrDefault(c => c.CategoryId == parentCategoryId);
        if (parent == null) return;
        parent.IsAddingSubcategory = !parent.IsAddingSubcategory; // Toggle visibility
        if (!parent.IsAddingSubcategory)
            parent.NewSubcategoryName = string.Empty; // Optionally clear input when hiding
        OnPropertyChanged(nameof(NonBillRootCategories));
        OnPropertyChanged(nameof(BillRootCategories));
    }
    
    // Command to save a new subcategory
    [RelayCommand]
    private void SaveSubcategory(CategoryModel parentCategory)
    {
        if (string.IsNullOrWhiteSpace(parentCategory.NewSubcategoryName))
            return;

        var newSubcategory = new CategoryModel
        {
            CategoryId = 0, 
            Name = parentCategory.NewSubcategoryName,
            ParentCategoryId = parentCategory.CategoryId,
            Income = 0, 
            Enabled = 1 
        };

        using (var context = new ApplicationDbContext())
        {
            context.Categories.Add(newSubcategory);
            context.SaveChanges();
        }

        // Update local Categories collection
        Categories.Add(newSubcategory);
        parentCategory.SubCategories = new ObservableCollection<CategoryModel>(Categories.Where(c => c.ParentCategoryId == parentCategory.CategoryId && c.Enabled == 1));
        
        // Reset input state
        parentCategory.IsAddingSubcategory = false;
        parentCategory.NewSubcategoryName = null;
        
        // Notify UI to refresh
        OnPropertyChanged(nameof(NonBillRootCategories));
        OnPropertyChanged(nameof(BillRootCategories));
    }
    
    public MainWindowViewModel()
    {
        // Set global connection string for design-time data context
        if (Design.IsDesignMode && string.IsNullOrEmpty((ApplicationDbContext.GlobalConnectionString)))
        {
            ApplicationDbContext.GlobalConnectionString =
                "Server=TSUNAMI\\SQLEXPRESS;Database=Budget2;Trusted_Connection=True;TrustServerCertificate=True;";
        }
        
        // Initialize collections
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
                .Where(t => t.CategoryId == category.CategoryId &&
                            t.Month == _currentDate.Month &&
                            t.Year == _currentDate.Year)
                .Sum(t => t.Amount);
        }

        // Calculate tri-monthly averages for each category
        foreach (var category in Categories)
        {
            var lastThreeMonths = Enumerable.Range(1, 3)
                .Select(i => _currentDate.AddMonths(-i))
                .Select(d => new { d.Month, d.Year })
                .ToList();

            category.TriMonthlyAvg = Convert.ToInt32(
                Transactions
                    .Where(t => t.CategoryId == category.CategoryId &&
                                lastThreeMonths.Any(m => t.Month == m.Month && t.Year == m.Year))
                    .Select(t => t.Amount)
                    .DefaultIfEmpty(0)
                    .Sum()/3
            );
        }
    }
}