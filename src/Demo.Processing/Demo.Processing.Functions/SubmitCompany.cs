using System;
using Azure.Storage.Queues.Models;
using Demo.Processing.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demo.Processing.Functions
{
    public class SubmitCompany
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SubmitCompany> _logger;

        public SubmitCompany(ApplicationDbContext dbContext, ILogger<SubmitCompany> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [Function(nameof(SubmitCompany))]
        public async Task Run([QueueTrigger("company-form", Connection = "FormsQueue")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");

            var form = message.Body.ToObjectFromJson<CommonForm>();

            if (form.Name == null)
            {
                _logger.LogError("Unable to deserialize form");
                return;
            }

            _logger.LogInformation($"Saving company {form.Name}");
            var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Name == form.Name);
            if (company != null)
            {
                company.ActiveDate = form.ActiveDate;
                company.MailingAddress = new MailingAddress
                {
                    Street = form.Street,
                    City = form.City,
                    State = form.State,
                    ZipCode = form.ZipCode,
                };
            }
            else
            {
                _dbContext.Companies.Add(new Company
                {
                    Name = form.Name,
                    ActiveDate = form.ActiveDate,
                    MailingAddress = new MailingAddress
                    {
                        Street = form.Street,
                        City = form.City,
                        State = form.State,
                        ZipCode = form.ZipCode,
                    },
                });
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
