using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PaymentsService.Data;
using PaymentsService.Models;

namespace PaymentsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentsDbContext _db;

    public PaymentsController(PaymentsDbContext db)
    {
        _db = db;
    }

    [HttpPost("create-account")]
    public async Task<IActionResult> CreateAccount(Guid userId)
    {
        if (_db.Accounts.Any(a => a.UserId == userId))
            return BadRequest("Account already exists.");

        var account = new Account { Id = Guid.NewGuid(), UserId = userId, Balance = 0 };
        await _db.Accounts.AddAsync(account);
        await _db.SaveChangesAsync();

        return Ok(account);
    }

    [HttpPost("top-up")]
    public async Task<IActionResult> TopUp(Guid userId, decimal amount)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (account == null) return NotFound();

        account.Balance += amount;
        await _db.SaveChangesAsync();

        return Ok(account);
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance(Guid userId)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (account == null) return NotFound();

        return Ok(new { account.UserId, account.Balance });
    }
}
