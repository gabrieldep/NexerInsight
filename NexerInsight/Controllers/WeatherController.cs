using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using NexerInsight.Services;

namespace NexerInsight.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherController(ILogger<WeatherController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet(Name = "GetData")]
        public IActionResult GetData()
        {
            BlobContainerClient containerClient = new AzureStorageService(_configuration).GetBlobContainerClient("iotbackend");
            BlobClient blobClient = containerClient.GetBlobClient("metadata.csv");

            var str = AzureStorageService.GetStreamFromBlobClient(blobClient);

            return Ok();
        }
    }
}