using ExpenseTracker.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Expense> Expenses { get; set; }
    }
}
