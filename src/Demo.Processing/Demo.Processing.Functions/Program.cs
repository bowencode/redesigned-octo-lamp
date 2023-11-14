using Demo.Processing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) =>
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(ctx.Configuration.GetValue<string>("SqlConnectionString"));
        });

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddQueueServiceClient(ctx.Configuration.GetConnectionString("FormsQueue"));
        });
    })
    .Build();

host.Run();
