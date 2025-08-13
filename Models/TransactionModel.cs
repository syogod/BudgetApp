using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BudgetApp.Models;

public class TransactionModel
{
    [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int transactionId { get; set; }
    public required int Month { get; set; }
    public required int Year { get; set; }
    public required int CategoryID { get; set; }
    public required int Amount { get; set; }
}