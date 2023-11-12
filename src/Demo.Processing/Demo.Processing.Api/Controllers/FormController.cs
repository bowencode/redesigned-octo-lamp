﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Processing.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FormController : ControllerBase
{
    [HttpPost]
    public IActionResult Post(FormRequest request)
    {
        return Ok(request);
    }
}

public class FormRequest
{
}
