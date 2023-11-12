using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Processing.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var response = new ReportResponse();
        return Ok(response);
    }
}

public class ReportResponse
{
    
}
