using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Demo.Processing.Functions
{
    public class SubmitIndividual
    {
        private readonly ILogger<SubmitIndividual> _logger;

        public SubmitIndividual(ILogger<SubmitIndividual> logger)
        {
            _logger = logger;
        }

        [Function(nameof(SubmitIndividual))]
        public void Run([QueueTrigger("individual-form", Connection = "FormsQueue")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
