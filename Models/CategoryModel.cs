using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetApp.Models;

public class CategoryModel
{
    [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public required int CategoryId { get; set; }
    public required string Name { get; set; }
    public int? ParentCategoryId { get; set; }
    public required byte Income { get; set; }
    public required byte Enabled { get; set; } = 1;
}