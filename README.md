# BudgetApp

A cross-platform personal budgeting application built with C#, Avalonia UI, and Entity Framework Core.

## Features

- Track income and expenses by category and subcategory
- Add transactions for each month
- View monthly and estimated summaries
- Bill and non-bill category separation
- Data stored in a local SQL Server database

## Technologies

- C# (.NET)
- Avalonia UI
- Entity Framework Core
- CommunityToolkit.Mvvm

## Getting Started

1. **Clone the repository:**
```bash
 git clone https://github.com/yourusername/BudgetApp.git
 ```
2. **Configure the database:**
    - Update the connection string in `Data/ApplicationDbContext.cs` if needed.

3. **Build and run:**
    - Open the solution in JetBrains Rider or Visual Studio.
    - Build and run the project.

## Project Structure

- `Models/` - Data models for categories and transactions
- `ViewModels/` - MVVM view models
- `Views/` - Avalonia XAML UI files
- `Data/` - Database context
- `Behaviors/` - UI behaviors (e.g., Enter key, numeric only)
- `Converters/` - Value converters for data binding
- `Styles/` - App-wide styles


## License

MIT License