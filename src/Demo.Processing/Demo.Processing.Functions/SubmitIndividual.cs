using System;
using Azure.Storage.Queues.Models;
using Demo.Processing.Data;
using Microsoft.Azure.Functions.Worker;
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
        public void Run([QueueTrigger("individual-form", Connection = "FormsQueue")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
