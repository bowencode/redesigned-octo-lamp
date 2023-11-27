using Newtonsoft.Json;
using Xunit.Abstractions;

namespace IntegrationTests;
public class CloudTestUtility
{
    public CloudTestUtility(ITestOutputHelper testOutputHelper)
    {
        Output = testOutputHelper;
    }

    public ITestOutputHelper Output { get; }

    public async Task<TfOutput?> LoadVariables()
    {
        var json = await File.ReadAllTextAsync(@"..\..\..\..\..\..\terraform\tf-output.json");
        Output.WriteLine($"Read TF Output: {json}");
        var tfOutput = JsonConvert.DeserializeObject<TfOutput>(json);
        return tfOutput;
    }
}