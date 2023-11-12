using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Demo.Processing.Functions
{
    public class SubmitCompany
    {
        private readonly ILogger<SubmitCompany> _logger;

        public SubmitCompany(ILogger<SubmitCompany> logger)
        {
            _logger = logger;
        }

        [Function(nameof(SubmitCompany))]
        public void Run([QueueTrigger("company-form", Connection = "FormsQueue")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
