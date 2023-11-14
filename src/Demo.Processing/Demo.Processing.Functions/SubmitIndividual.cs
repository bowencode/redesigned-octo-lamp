using System;
using Azure.Storage.Queues.Models;
using Demo.Processing.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demo.Processing.Functions
{
    public class SubmitIndividual
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SubmitIndividual> _logger;

        public SubmitIndividual(ApplicationDbContext dbContext, ILogger<SubmitIndividual> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [Function(nameof(SubmitIndividual))]
        public async Task Run([QueueTrigger("individual-form", Connection = "FormsQueue")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");

            var form = message.Body.ToObjectFromJson<CommonForm>();

            if (form.Name == null)
            {
                _logger.LogError("Unable to deserialize form");
                return;
            }

            _logger.LogInformation($"Saving individual {form.Name}");
            var user = await _dbContext.Users.FirstOrDefaultAsync(c => c.Name == form.Name);
            if (user != null)
            {
                user.ActiveDate = DateTime.UtcNow;
                user.MailingAddress = new MailingAddress
                {
                    Street = form.Street,
                    City = form.City,
                    State = form.State,
                    ZipCode = form.ZipCode,
                };
            }
            else
            {
                _dbContext.Users.Add(new User
                {
                    Name = form.Name,
                    ActiveDate = DateTime.UtcNow,
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
