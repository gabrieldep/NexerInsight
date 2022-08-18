using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace NexerInsight.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherController(ILogger<WeatherController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet(Name = "GetWeather")]
        public async Task<IEnumerable<Weather>> GetAsync()
        {
            string connectionString = _configuration.GetConnectionString("AzureStorage");
            BlobServiceClient blobServiceClient = new(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("iotbackend");
            BlobClient blobClient = containerClient.GetBlobClient("metadata.csv");
            string localFilePath = Path.Combine("C:\\Users\\gabri\\source\\repos\\Pessoal", "metadata.csv");

            await blobClient.DownloadToAsync(localFilePath);

            return Enumerable.Range(1, 5).Select(index => new Weather
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}