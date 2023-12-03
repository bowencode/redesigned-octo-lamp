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
    public async Task SubmittedForm_ForIndividual_IsProcessed()
    {
        var utility = new CloudTestUtility(Output);
        var variables = await utility.LoadVariables();

        Assert.NotNull(variables);

        string? token = await GetAuthToken(variables);

        var dataGenerator = new Faker();
        string stateCode = dataGenerator.Address.StateAbbr();
        string personName = dataGenerator.Name.FullName();

        ReportResponse? reportData = await SubmitAndVerifyForm(variables, token, personName, stateCode);

        Assert.NotNull(reportData?.Users);
        Assert.True(reportData.Users.TryGetValue(personName, out var state));
        Assert.Equal(stateCode, state);
    }

    private async Task<string?> GetAuthToken(TfOutput variables)
    {
        Assert.NotNull(variables.TestAppClientId?.Value);
        Assert.NotNull(variables.TestAppClientSecret?.Value);
        Assert.NotNull(variables.ApiAppAuthority?.Value);
        Assert.NotNull(variables.ApiAppScope?.Value);

        var tokenClient = new HttpClient();
        var tokenResponse = await tokenClient.PostAsync($"{variables.ApiAppAuthority.Value}/oauth2/v2.0/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = variables.TestAppClientId.Value,
            ["client_secret"] = variables.TestAppClientSecret.Value,
            ["scope"] = variables.ApiAppScope.Value,
            ["grant_type"] = "client_credentials"
        }));

        var token = JObject.Parse(await tokenResponse.Content.ReadAsStringAsync())["access_token"]?.Value<string>();
        Output.WriteLine($"Received Token: {token}");
        Assert.NotNull(token);
        return token;
    }

    private static async Task<ReportResponse?> GetReportData(TfOutput variables, HttpClient client)
    {
        Assert.NotNull(variables.ApiAppUrl?.Value);

        var reportResponse = await client.GetAsync($"{variables.ApiAppUrl.Value}/api/report");
        Assert.NotNull(reportResponse);
        Assert.True(reportResponse.IsSuccessStatusCode);

        var reportData = await reportResponse.Content.ReadFromJsonAsync<ReportResponse>();
        return reportData;
    }

    private async Task<ReportResponse?> SubmitAndVerifyForm(TfOutput variables, string? token, string name, string stateCode, bool individual = true)
    {
        Assert.NotNull(variables.ApiAppUrl?.Value);

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var reportData = await GetReportData(variables, client);
        int initialCount;
        if (individual)
        {
            Assert.NotNull(reportData?.Users);
            initialCount = reportData.Users.Count;
        }
        else
        {
            Assert.NotNull(reportData?.Companies);
            initialCount = reportData.Companies.Count;
        }
        Output.WriteLine($"Initial Count: {initialCount}");

        await SubmitForm(variables, name, stateCode, individual, client);

        Output.WriteLine($"Waiting for form to be processed");
        await Task.Delay(5000);

        var updatedCount = initialCount;
        int counter = 0;
        while (updatedCount == initialCount && counter++ < 30)
        {
            await Task.Delay(2000);
            Output.WriteLine($"Checking for processed form ({counter})");
            reportData = await GetReportData(variables, client);
            if (individual)
            {
                Assert.NotNull(reportData?.Users);
                updatedCount = reportData.Users.Count;
            }
            else
            {
                Assert.NotNull(reportData?.Companies);
                updatedCount = reportData.Companies.Count;
            }
        }

        Output.WriteLine($"Updated Count: {updatedCount}");
        Assert.NotEqual(initialCount, updatedCount);
        return reportData;
    }

    private async Task SubmitForm(TfOutput variables, string name, string stateCode, bool individual, HttpClient client)
    {
        Output.WriteLine($"Submitting form for {name} in {stateCode}");
        var dataGenerator = new Faker();
        var formResponse = await client.PostAsync($"{variables.ApiAppUrl.Value}/api/form", new StringContent(JsonConvert.SerializeObject(new FormRequest
        {
            Name = name,
            ActiveDate = dataGenerator.Date.PastOffset(5).Date,
            IsIndividual = individual,
            Street = dataGenerator.Address.StreetAddress(),
            City = dataGenerator.Address.City(),
            State = stateCode,
            ZipCode = dataGenerator.Address.ZipCode()
        }), Encoding.UTF8, "application/json"));

        Assert.NotNull(formResponse);
        Assert.True(formResponse.IsSuccessStatusCode);
    }

    [Fact]
    public async Task SubmittedForm_ForLondonCompany_IsProcessed()
    {
        var utility = new CloudTestUtility(Output);
        var variables = await utility.LoadVariables();

        Assert.NotNull(variables);

        string? token = await GetAuthToken(variables);

        var dataGenerator = new Faker();
        string stateCode = "LND";
        string personName = dataGenerator.Name.FullName();

        ReportResponse? reportData = await SubmitAndVerifyForm(variables, token, personName, stateCode, false);

        Assert.NotNull(reportData?.Companies);
        Assert.True(reportData.Companies.TryGetValue(personName, out var state));
        Assert.Equal(stateCode, state);
    }
}
