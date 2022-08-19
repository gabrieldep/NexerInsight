using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using NexerInsight.Models;
using NexerInsight.Services;
using System.Net;
using static NexerInsight.Models.Enums;

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

        [HttpGet("GetData")]
        public IActionResult GetData(DateTime date, string sensorType)
        {
            if (!Enum.TryParse(typeof(SensorType), sensorType.ToLower(), out _))
                return StatusCode((int)HttpStatusCode.BadRequest, new { message = "This sensorType doesn't exists" });
            BlobContainerClient containerClient = new AzureStorageService(_configuration).GetBlobContainerClient("iotbackend");
            BlobClient blobClient = containerClient.GetBlobClient($"dockan/{sensorType}/{date:yyyy-MM-dd}.csv");
            Stream str = AzureStorageService.GetStreamFromBlobClient(blobClient);
            var dados = ArchiveService.GetArrayFromStream(str);
            return StatusCode((int)HttpStatusCode.OK, dados);
        }
    }
}