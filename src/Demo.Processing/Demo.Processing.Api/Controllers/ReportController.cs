using Demo.Processing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Processing.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public ReportController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var allCompanies = await _dbContext.Companies
            .OrderByDescending(c => c.ActiveDate)
            .Select(u => new { Name = u.Name, State = u.MailingAddress != null ? u.MailingAddress.State : null })
            .ToListAsync();
        var allUsers = await _dbContext.Users
            .OrderByDescending(u => u.ActiveDate)
            .Select(u => new { Name = u.Name, State = u.MailingAddress != null ? u.MailingAddress.State : null })
            .ToListAsync();

        var response = new ReportResponse
        {
            Companies = allCompanies.Distinct().ToDictionary(e => e.Name, e => e.State),
            Users = allUsers.Distinct().ToDictionary(e => e.Name, e => e.State),
        };
        return Ok(response);
    }
}

public class ReportResponse
{
    public Dictionary<string, string?>? Companies { get; set; }
    public Dictionary<string, string?>? Users { get; set; }
}
