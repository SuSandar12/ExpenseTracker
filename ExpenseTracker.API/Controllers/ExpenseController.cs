using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExpenseController : ControllerBase
{
    [Authorize]
    [HttpGet("expenses")]
    public IActionResult GetExpenses()
    {
        // Logic to retrieve expenses for the authenticated user
        return Ok(new { message = "List of expenses" });
    }
}
