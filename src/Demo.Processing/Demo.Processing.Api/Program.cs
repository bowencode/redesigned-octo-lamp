using Azure.Identity;
using Demo.Processing.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;

namespace Demo.Processing.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
            var loggerFactory = LoggerFactory.Create(logging => logging.SetMinimumLevel(LogLevel.Debug).AddConsole());
            var startupLogger = loggerFactory.CreateLogger<Program>();
            var credentialOptions = new DefaultAzureCredentialOptions();

            string? kvUrlSetting = builder.Configuration.GetValue<string>("KeyVaultUrl");
            if (!string.IsNullOrEmpty(kvUrlSetting))
            {
                try
                {
                    builder.Configuration.AddAzureKeyVault(
                        new Uri(kvUrlSetting),
                        new DefaultAzureCredential(credentialOptions));

                    startupLogger.LogDebug("KeyVault added: {KeyVaultUrl}", kvUrlSetting);
                }
                catch (Exception ex)
                {
                    startupLogger.LogError(ex, "Failed to add KeyVault");
                }
            }
        }

        // Add services to the container.
        builder.Services.AddControllers();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetValue<string>("SqlConnectionString"), sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(3);
            });
        });

        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(builder.Configuration.GetValue<string>("FormsStorage"));
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration.GetValue<string>("AzureAd:Authority");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudiences = new[] { builder.Configuration.GetValue<string>("AzureAd:Audience") }
                };
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

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
