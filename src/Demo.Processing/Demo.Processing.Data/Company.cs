namespace Demo.Processing.Data;

public class Company
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime ActiveDate { get; set; }
    public MailingAddress? MailingAddress { get; set; }
}
