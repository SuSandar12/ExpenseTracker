using ExpenseTracker.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(AppDbContext context, ILogger<ExpenseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var claimValue = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return !string.IsNullOrEmpty(claimValue) && int.TryParse(claimValue, out userId);
        }

        [HttpGet("expenses")]
        public async Task<IActionResult> GetExpenses()
        {
            if (!TryGetUserId(out var userId))
            {
                _logger.LogWarning("GetExpenses: missing or invalid user id claim.");
                return Unauthorized();
            }

            _logger.LogInformation("GetExpenses: loading expenses for user {UserId}", userId);

            var expenses = await _context.Expenses
                .AsNoTracking()
                .Where(e => e.UserId == userId)
                .ToListAsync();

            _logger.LogDebug("GetExpenses: returned {Count} expenses for user {UserId}", expenses.Count, userId);
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpense(int id)
        {
            if (!TryGetUserId(out var userId))
            {
                _logger.LogWarning("GetExpense({Id}): missing or invalid user id claim.", id);
                return Unauthorized();
            }

            _logger.LogInformation("GetExpense({Id}): user {UserId} requested", id, userId);

            var expense = await _context.Expenses
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (expense == null)
            {
                _logger.LogInformation("GetExpense({Id}): not found for user {UserId}", id, userId);
                return NotFound();
            }

            return Ok(expense);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] ExpenseCreateDto dto)
        {
            if (!TryGetUserId(out var userId))
            {
                _logger.LogWarning("CreateExpense: missing or invalid user id claim.");
                return Unauthorized();
            }

            if (dto == null)
            {
                _logger.LogWarning("CreateExpense: invalid payload from user {UserId}", userId);
                return BadRequest();
            }

            var expense = new Expense
            {
                Title = dto.Title,
                Amount = dto.Amount,
                Date = dto.Date,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("CreateExpense: user {UserId} creating expense {@Expense}", userId, expense);

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExpense), new { id = expense.Id }, expense);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] ExpenseUpdateDto dto)
        {
            if (!TryGetUserId(out var userId))
            {
                _logger.LogWarning("UpdateExpense({Id}): missing or invalid user id claim.", id);
                return Unauthorized();
            }

            if (dto == null)
            {
                _logger.LogWarning("UpdateExpense({Id}): invalid payload from user {UserId}", id, userId);
                return BadRequest();
            }

            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (expense == null)
            {
                _logger.LogInformation("UpdateExpense({Id}): not found for user {UserId}", id, userId);
                return NotFound("Expense not found");
            }

            expense.Title = dto.Title;
            expense.Amount = dto.Amount;
            expense.Date = dto.Date;

            _logger.LogInformation("UpdateExpense({Id}): updating expense for user {UserId}", id, userId);

            await _context.SaveChangesAsync();

            return Ok(expense);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            if (!TryGetUserId(out var userId))
            {
                _logger.LogWarning("DeleteExpense({Id}): missing or invalid user id claim.", id);
                return Unauthorized();
            }

            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (expense == null)
            {
                _logger.LogInformation("DeleteExpense({Id}): not found for user {UserId}", id, userId);
                return NotFound();
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            _logger.LogInformation("DeleteExpense({Id}): deleted by user {UserId}", id, userId);
            return NoContent();
        }
    }
}
