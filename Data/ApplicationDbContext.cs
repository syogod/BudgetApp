using System;
using Microsoft.EntityFrameworkCore;
using BudgetApp.Models;

namespace BudgetApp.Data;

public class ApplicationDbContext: DbContext
{
    public static string? GlobalConnectionString { get; set; } = null;
    private readonly string _connectionString;
    
    public DbSet<CategoryModel>  Categories { get; set; }
    public DbSet<TransactionModel> Transactions { get; set; }
    
    public ApplicationDbContext()
    {
        if (GlobalConnectionString == null)
        {
            System.Diagnostics.Debug.WriteLine("ApplicationDbContext constructed before GlobalConnectionString was set!");
            throw new InvalidOperationException("GlobalConnectionString is not set.");
        }
        _connectionString = GlobalConnectionString;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
}