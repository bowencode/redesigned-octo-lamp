using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using Demo.Processing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Processing.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FormController : ControllerBase
{
    private readonly BlobServiceClient _blobService;

    public FormController(BlobServiceClient blobService)
    {
        _blobService = blobService;
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

        var blobContainer = _blobService.GetBlobContainerClient("received-forms");
        string fileName = $"{form.Name}-{Guid.NewGuid()}.json";
        var client = blobContainer.GetBlobClient(fileName);
        await client.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(form))));

        return Ok(fileName);
    }
}
