using Demo.Processing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

namespace Demo.Processing.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetValue<string>("SqlConnectionString"));
        });

        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(builder.Configuration.GetConnectionString("FormsStorage"));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
