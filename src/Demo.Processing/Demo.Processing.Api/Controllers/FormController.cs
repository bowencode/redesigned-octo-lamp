using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using Demo.Processing.Data;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Processing.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FormController : ControllerBase
{
    private readonly BlobContainerClient _blobContainer;

    public FormController(BlobContainerClient blobContainer)
    {
        _blobContainer = blobContainer;
    }

    [HttpPost]
    public async Task<IActionResult> Post(FormRequest request)
    {
        var form = new CommonForm
        {
            Name = request.Name,
            ActiveDate = request.ActiveDate ?? DateTime.MinValue,
            SubmissionType = request.IsIndividual ? SubmissionType.Individual : SubmissionType.Company,
            Street = request.Street,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
        };

        string fileName = $"{form.Name}-{form.ActiveDate:yyyyMMdd}.json";
        var client = _blobContainer.GetBlobClient(fileName);
        await client.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(form))));

        return Ok(fileName);
    }
}

public class FormRequest
{
    public bool IsIndividual { get; set; }
    public string? Name { get; set; }
    public DateTime? ActiveDate { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
}
