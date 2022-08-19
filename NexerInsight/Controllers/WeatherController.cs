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
        private readonly AzureStorageService _azureService;
        private readonly BlobContainerClient _containerClient;

        public WeatherController(IConfiguration configuration)
        {
            _azureService = new AzureStorageService(configuration);
            _containerClient = _azureService.GetBlobContainerClient("iotbackend");
        }

        [HttpGet("GetData")]
        public IActionResult GetData(DateTime date, string sensorType, string deviceId)
        {
            if (!Enum.TryParse(typeof(SensorType), sensorType.ToLower(), out _))
                return StatusCode((int)HttpStatusCode.BadRequest, new { message = "This sensorType doesn't exists" });

            var blobClient = _containerClient.GetBlobClient($"{deviceId}/{sensorType}/{date:yyyy-MM-dd}.csv");
            if (blobClient.Exists())
                return StatusCode((int)HttpStatusCode.OK, ArchiveService.GetArrayFromStream(blobClient.OpenRead()));

            Stream str = AzureStorageService.GetHistoricalStream(deviceId, sensorType, _containerClient);
            using ZipArchive package = new(str, ZipArchiveMode.Read);
            ZipArchiveEntry? a = package.Entries.FirstOrDefault(e => e.Name == $"{date:yyyy-MM-dd}.csv");

            if (a == null)
                return StatusCode((int)HttpStatusCode.NotFound, new { message = "This info was not found on our historical" });
            return StatusCode((int)HttpStatusCode.OK, ArchiveService.GetArrayFromStream(a.Open()));
        }

        [HttpGet("GetDataForDevice")]
        public IActionResult GetDataForDevice(DateTime date)
        {
            return StatusCode((int)HttpStatusCode.OK);
        }
    }
}