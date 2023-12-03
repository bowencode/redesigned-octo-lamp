using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using Demo.Processing.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Demo.Processing.Functions
{
    public class ParseForm
    {
        private readonly QueueServiceClient _queueService;
        private readonly ILogger<ParseForm> _logger;

        public ParseForm(QueueServiceClient queueService, ILogger<ParseForm> logger)
        {
            _queueService = queueService;
            _logger = logger;
        }

        [Function(nameof(ParseForm))]
        public async Task Run([BlobTrigger("received-forms/{name}", Connection = "FormsStorage")] Stream stream, string name)
        {
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name:{name}");
            var form = await JsonSerializer.DeserializeAsync<CommonForm>(stream);

            if (form is null)
            {
                _logger.LogError("Unable to deserialize form");
                return;
            }

            try
            {
                var queueName = GetQueueName(form);
                _logger.LogInformation($"Sending to {queueName} queue");
                await _queueService.GetQueueClient(queueName).SendMessageAsync(AsMessage(form));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to send message to queue");
                throw;
            }
        }

        private static string AsMessage(CommonForm form)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(form)));
        }

        private static string GetQueueName(CommonForm form)
        {
            return form.SubmissionType switch
            {
                SubmissionType.Company => "company-forms",
                SubmissionType.Individual => "individual-form",
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
