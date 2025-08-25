using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BudgetApp.Models;

public partial class CategoryModel : ObservableObject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required int CategoryId { get; set; }

    public required string Name { get; set; }
    public int? ParentCategoryId { get; init; }
    public required byte Income { get; init; }
    public required byte Enabled { get; init; } = 1;

    [NotMapped] public ObservableCollection<CategoryModel>? SubCategories { get; set; }

    private int _monthlySum = 0;
    private int _triMonthlyAvg = 0;
    private int? _newTransactionAmount = null;

    // MonthlySum is the total amount for this category for the current month
    // Setting MonthlySum will notify the Parent category to update its MonthlyParentCategorySum
    [NotMapped]
    public int MonthlySum
    {
        get => _monthlySum;
        set
        {
            if (SetProperty(ref _monthlySum, value))
            {
                Parent?.OnPropertyChanged(nameof(MonthlyParentCategorySum));
            }
        }
    }

    // TriMonthlyAvg is the average monthly amount for this category over the last three months
    // Setting TriMonthlyAvg will notify the Parent category to update its MonthlyParentCategoryEst
    [NotMapped]
    public int TriMonthlyAvg
    {
        get => _triMonthlyAvg;
        set => SetProperty(ref _triMonthlyAvg, value);
    }

    // NewTransactionAmount is a temporary property used for adding a new transaction
    // It is not mapped to the database
    [NotMapped]
    public int? NewTransactionAmount
    {
        get => _newTransactionAmount;
        set => SetProperty(ref _newTransactionAmount, value);
    }

    // IsAddingSubcategory indicates whether the UI is in the state of adding a new subcategory
    [NotMapped] public bool IsAddingSubcategory { get; set; } = false;

    [NotMapped] public string? NewSubcategoryName { get; set; } = null;

    [NotMapped] public CategoryModel? Parent { get; init; }

    // MonthlyParentCategorySum is the sum of MonthlySum of all subcategories
    // It is computed dynamically and not stored in the database
    [NotMapped]
    public int MonthlyParentCategorySum
    {
        get { return SubCategories?.Sum(sc => sc.MonthlySum) ?? 0; }
    }

    // MonthlyParentCategoryEst is the estimated monthly amount for the parent category
    // For income categories, it uses the higher of TriMonthlyAvg or MonthlySum from subcategories
    // For expense categories, it uses the higher of MonthlySum or TriMonthlyAvg from subcategories
    // It is computed dynamically and not stored in the database
    [NotMapped]
    public int MonthlyParentCategoryEst
    {
        get
        {
            if (Income == 1)
            {
                return SubCategories?.Sum(sc => sc.TriMonthlyAvg > sc.MonthlySum ? sc.TriMonthlyAvg : sc.MonthlySum) ??
                       0;
            }
            else
            {
                return SubCategories?.Sum(sc => Math.Max(sc.MonthlySum, sc.TriMonthlyAvg)) ?? 0;
            }
        }
    }
}