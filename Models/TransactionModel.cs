namespace BudgetApp.Models;

public class TransactionModel
{
    public required int Month { get; set; }
    public required int Year { get; set; }
    public required string Category { get; set; }
    public required int Amount { get; set; }
}