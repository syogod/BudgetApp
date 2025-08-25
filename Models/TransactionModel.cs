using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BudgetApp.Models;

public class TransactionModel
{
    [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int TransactionId { get; init; }
    public required int Month { get; init; }
    public required int Year { get; init; }
    public required int CategoryId { get; init; }
    public required int Amount { get; init; }
}