using Microsoft.EntityFrameworkCore;
using BudgetApp.Models;

namespace BudgetApp.Data;

public class ApplicationDbContext: DbContext
{
    public DbSet<CategoryModel>  Categories { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=TSUNAMI\\SQLEXPRESS;Database=Budget2;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}