using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Demo.Processing.Functions
{
    public class ParseForm
    {
        private readonly ILogger<ParseForm> _logger;

        public ParseForm(ILogger<ParseForm> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ParseForm))]
        public async Task Run([BlobTrigger("received/{name}", Connection = "FormsStorage")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        }
    }
}
