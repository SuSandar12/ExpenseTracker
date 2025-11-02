namespace ExpenseTracker.API.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Category { get; set; } = string.Empty;
    }
}
