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
    public int? ParentCategoryId { get; set; }
    public required byte Income { get; set; }
    public required byte Enabled { get; set; } = 1;

    [NotMapped] public ObservableCollection<CategoryModel>? SubCategories { get; set; }

    private int _monthlySum = 0;
    private int _triMonthlyAvg = 0;
    private int? _newTransactionAmount = null;

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


    [NotMapped]
    public int TriMonthlyAvg
    {
        get => _triMonthlyAvg;
        set => SetProperty(ref _triMonthlyAvg, value);
    }

    [NotMapped]
    public int? NewTransactionAmount
    {
        get => _newTransactionAmount;
        set => SetProperty(ref _newTransactionAmount, value);
    }

    [NotMapped] public bool IsAddingSubcategory { get; set; } = false;

    [NotMapped] public string? NewSubcategoryName { get; set; } = null;

    [NotMapped] public CategoryModel? Parent { get; set; }

    [NotMapped]
    public int MonthlyParentCategorySum
    {
        get { return SubCategories?.Sum(sc => sc.MonthlySum) ?? 0; }
    }

    [NotMapped]
    public int MonthlyParentCategoryEst
    {
        get
        {
            if (Income == 1)
            {
                // For income: use TriMonthlyAvg if greater than MonthlySum
                return SubCategories?.Sum(sc => sc.TriMonthlyAvg > sc.MonthlySum ? sc.TriMonthlyAvg : sc.MonthlySum) ??
                       0;
            }
            else
            {
                // For expenses: use MonthlySum if greater than TriMonthlyAvg
                return SubCategories?.Sum(sc => Math.Max(sc.MonthlySum, sc.TriMonthlyAvg)) ?? 0;
            }
        }
    }
}