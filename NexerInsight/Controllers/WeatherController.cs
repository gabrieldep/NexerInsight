using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using NexerInsight.Models;
using NexerInsight.Services;
using System.IO.Compression;
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
        public IActionResult GetData(DateTime date, string sensorType, string deviceId)
        {
            var azureService = new AzureStorageService(_configuration);

            if (!Enum.TryParse(typeof(SensorType), sensorType.ToLower(), out _))
                return StatusCode((int)HttpStatusCode.BadRequest, new { message = "This sensorType doesn't exists" });
            BlobContainerClient containerClient = azureService.GetBlobContainerClient("iotbackend");
            var blobClient = containerClient.GetBlobClient($"{deviceId}/{sensorType}/{date:yyyy-MM-dd}.csv");
            if (blobClient.Exists())
            {
                Stream strFile = AzureStorageService.GetStreamFromBlobClient(blobClient);
                var dados = ArchiveService.GetArrayFromStream(strFile);
                return StatusCode((int)HttpStatusCode.OK, dados);
            }

            blobClient = containerClient.GetBlobClient($"{deviceId}/{sensorType}/historical.zip");
            Stream str = AzureStorageService.GetStreamFromBlobClient(blobClient);
            using ZipArchive package = new(str, ZipArchiveMode.Read);
            ZipArchiveEntry? a = package.Entries.FirstOrDefault(e => e.Name == $"{date:yyyy-MM-dd}.csv");
            if (a == null)
                return StatusCode((int)HttpStatusCode.NotFound, new {message = "This info was not found on our historical"});

            Stream fileStream = a.Open();
            return StatusCode((int)HttpStatusCode.OK, ArchiveService.GetArrayFromStream(fileStream));
        }

        [HttpGet("GetDataForDevice")]
        public IActionResult GetDataForDevice(DateTime date)
        {
            BlobContainerClient containerClient = new AzureStorageService(_configuration).GetBlobContainerClient("iotbackend");

            BlobClient blobClient = containerClient.GetBlobClient($"dockan/humidity/historical.zip");
            Stream str = AzureStorageService.GetStreamFromBlobClient(blobClient);
            using ZipArchive package = new(str, ZipArchiveMode.Read);
            ZipArchiveEntry? a = package.Entries.First(e => e.Name == $"{date:yyyy-MM-dd}.csv");
            var fileStream = a.Open();
            ArchiveService.GetArrayFromStream(fileStream);
            return StatusCode((int)HttpStatusCode.OK, ArchiveService.GetArrayFromStream(fileStream));
        }
    }
}