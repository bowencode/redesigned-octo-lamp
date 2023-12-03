namespace Demo.Processing.Data;

public class CommonForm
{
    public string? Name { get; set; }
    public DateTime ActiveDate { get; set; }
    public SubmissionType SubmissionType { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
}

public enum SubmissionType
{
    Company,
    Individual,
}