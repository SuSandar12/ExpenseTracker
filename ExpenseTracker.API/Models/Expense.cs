using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.API.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User? User { get; set; }
    }
}
