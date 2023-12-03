namespace IntegrationTests;

public class TfOutput
{
    public TfValue? ApiAppUrl { get; set; }
    public TfValue? SqlConnectionString { get; set; }
    public TfValue? StorageConnectionString { get; set; }
    public TfValue? TestAppClientId { get; set; }
    public TfValue? TestAppClientSecret { get; set; }
    public TfValue? ApiAppAuthority { get; set; }
    public TfValue? ApiAppAudience { get; set; }
    public TfValue? ApiAppScope { get; set; }
}

public class TfValue
{
    public string? Value { get; set; }
}
