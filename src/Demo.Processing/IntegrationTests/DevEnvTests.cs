using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using Bogus;
using Demo.Processing.Api.Controllers;
using Xunit.Abstractions;
using System.Net.Http.Json;

namespace IntegrationTests;

public class DevEnvTests
{
    public ITestOutputHelper Output { get; }

    public DevEnvTests(ITestOutputHelper testOutputHelper)
    {
        Output = testOutputHelper;
    }

    [Fact]
    public async Task SubmittedFormIsProcessed()
    {
        var utility = new CloudTestUtility(Output);
        var variables = await utility.LoadVariables();

        Assert.NotNull(variables?.ApiAppUrl?.Value);
        Assert.NotNull(variables.TestAppClientId?.Value);
        Assert.NotNull(variables.TestAppClientSecret?.Value);
        Assert.NotNull(variables.ApiAppAuthority?.Value);
        Assert.NotNull(variables.ApiAppScope?.Value);

        var client = new HttpClient();
        var tokenResponse = await client.PostAsync($"{variables.ApiAppAuthority.Value}/oauth2/v2.0/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = variables.TestAppClientId.Value,
            ["client_secret"] = variables.TestAppClientSecret.Value,
            ["scope"] = variables.ApiAppScope.Value,
            ["grant_type"] = "client_credentials"
        }));

        var token = JObject.Parse(await tokenResponse.Content.ReadAsStringAsync())["access_token"]?.Value<string>();
        Output.WriteLine($"Received Token: {token}");
        Assert.NotNull(token);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var reportData = await GetReportData(variables, client);
        Assert.NotNull(reportData?.Users);
        var userCount = reportData.Users.Count;
        Output.WriteLine($"Initial User Count: {userCount}");

        var dataGenerator = new Faker();
        string stateCode = dataGenerator.Address.StateAbbr();
        string personName = dataGenerator.Name.FullName();
        Output.WriteLine($"Submitting form for {personName} in {stateCode}");
        var formResponse = await client.PostAsync($"{variables.ApiAppUrl.Value}/api/form", new StringContent(JsonConvert.SerializeObject(new FormRequest
        {
            Name = personName,
            ActiveDate = dataGenerator.Date.PastOffset(5).Date,
            IsIndividual = true,
            Street = dataGenerator.Address.StreetAddress(),
            City = dataGenerator.Address.City(),
            State = stateCode,
            ZipCode = dataGenerator.Address.ZipCode()
        }), Encoding.UTF8, "application/json"));

        Assert.NotNull(formResponse);
        Assert.True(formResponse.IsSuccessStatusCode);

        Output.WriteLine($"Waiting for form to be processed");
        await Task.Delay(5000);

        var updatedUserCount = userCount;
        int counter = 0;
        while (updatedUserCount == userCount && counter++ < 60)
        {
            await Task.Delay(2000);
            Output.WriteLine($"Checking for processed form ({counter})");
            reportData = await GetReportData(variables, client);
            Assert.NotNull(reportData?.Users);
            updatedUserCount = reportData.Users.Count;
        }

        Output.WriteLine($"Updated User Count: {updatedUserCount}");
        Assert.NotEqual(userCount, updatedUserCount);

        Assert.True(reportData.Users.TryGetValue(personName, out var state));
        Assert.Equal(stateCode, state);
    }

    private static async Task<ReportResponse?> GetReportData(TfOutput? variables, HttpClient client)
    {
        var reportResponse = await client.GetAsync($"{variables.ApiAppUrl.Value}/api/report");
        Assert.NotNull(reportResponse);
        Assert.True(reportResponse.IsSuccessStatusCode);

        var reportData = await reportResponse.Content.ReadFromJsonAsync<ReportResponse>();
        return reportData;
    }
}
