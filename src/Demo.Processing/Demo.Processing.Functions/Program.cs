using Demo.Processing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureHostConfiguration(config =>
    {
        config.AddUserSecrets<Program>();
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(ctx.Configuration.GetValue<string>("SqlConnectionString"), sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(3);
            });
        });

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddQueueServiceClient(ctx.Configuration.GetValue<string>("FormsQueue"));
        });
    })
    .Build();

host.Run();
