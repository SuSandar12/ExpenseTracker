using ExpenseTracker.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.API.Models;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExpenseController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExpenseController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("expenses")]
    public IActionResult GetExpenses()
    {
        var userId = int.Parse(User.FindFirst("id")?.Value);
        var expense = _context.Expenses.Where(e => e.UserId == userId).ToList();
        return Ok(expense);
    }

    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetExpense(int id)
    {
        var userId = int.Parse(User.FindFirst("id")?.Value!);
        var expense = _context.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == userId);
        if (expense == null) return NotFound();
        return Ok(expense);
    }

    [Authorize]
    [HttpPost]
    public IActionResult CreateExpense([FromBody] ExpenseCreateDto dto)
    {
        var userId = int.Parse(User.FindFirst("id").Value); // get from JWT

        var expense = new Expense
        {
            Title = dto.Title,
            Amount = dto.Amount,
            Date = dto.Date,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Expenses.Add(expense);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetExpense), new { id = expense.Id }, expense);
    }

    [Authorize]
    [HttpPut("{id}")]
    public IActionResult UpdateExpense(int id, [FromBody] ExpenseUpdateDto dto)
    {
        var userId = int.Parse(User.FindFirst("id").Value);

        var expense = _context.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == userId);
        if (expense == null)
            return NotFound("Expense not found");

        expense.Title = dto.Title;
        expense.Amount = dto.Amount;
        expense.Date = dto.Date;

        _context.SaveChanges();

        return Ok(expense);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult DeleteExpense(int id)
    {
        var userId = int.Parse(User.FindFirst("id")?.Value!);
        var expense = _context.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == userId);
        if (expense == null) return NotFound();

        _context.Expenses.Remove(expense);
        _context.SaveChanges();
        return NoContent();
    }
}
