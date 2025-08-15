using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BudgetApp.Models;

public partial class CategoryModel : ObservableObject
{
    [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public required int CategoryId { get; set; }
    public required string Name { get; set; }
    public int? ParentCategoryId { get; set; }
    public required byte Income { get; set; }
    public required byte Enabled { get; set; } = 1;

    [NotMapped] public ICollection<CategoryModel>? SubCategories { get; set; }

    private int _monthlySum = 0;
    private int _newTransactionAmount = 0;

    [NotMapped]
    public int MonthlySum
    {
        get => _monthlySum;
        set => SetProperty(ref _monthlySum, value);
    }

    [NotMapped]
    public int NewTransactionAmount
    {
        get => _newTransactionAmount;
        set => SetProperty(ref _newTransactionAmount, value);
    }
    
    [NotMapped]
    public bool IsAddingSubcategory { get; set; } = false;
    
    [NotMapped]
    public string? NewSubcategoryName { get; set; } = null;
}