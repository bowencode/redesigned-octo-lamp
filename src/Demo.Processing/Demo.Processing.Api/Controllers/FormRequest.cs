namespace Demo.Processing.Api.Controllers;

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
