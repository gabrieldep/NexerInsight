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

            var blobClient = _azureService.GetBlobClient(deviceId, sensorType, date, _containerClient);
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
        public IActionResult GetDataForDevice(DateTime date, string deviceId)
        {
            IEnumerable<SensorReading> temperature = new List<SensorReading>();
            IEnumerable<SensorReading> rain = new List<SensorReading>();
            IEnumerable<SensorReading> humidity = new List<SensorReading>();

            var tempData = _azureService.GetBlobClient(deviceId, "temperature", date, _containerClient);
            if (tempData.Exists())
                temperature = ArchiveService.GetArrayFromStream(tempData.OpenRead());
            else
            {
                Stream str = AzureStorageService.GetHistoricalStream(deviceId, "temperature", _containerClient);
                using ZipArchive package = new(str, ZipArchiveMode.Read);
                ZipArchiveEntry? a = package.Entries.FirstOrDefault(e => e.Name == $"{date:yyyy-MM-dd}.csv");
                temperature = ArchiveService.GetArrayFromStream(a.Open());
            }

            var rainData = _azureService.GetBlobClient(deviceId, "rainfall", date, _containerClient);
            if (rainData.Exists())
                rain = ArchiveService.GetArrayFromStream(rainData.OpenRead());
            else
            {
                Stream str = AzureStorageService.GetHistoricalStream(deviceId, "rainfall", _containerClient);
                using ZipArchive package = new(str, ZipArchiveMode.Read);
                ZipArchiveEntry? a = package.Entries.FirstOrDefault(e => e.Name == $"{date:yyyy-MM-dd}.csv");
                rain = ArchiveService.GetArrayFromStream(a.Open());
            }

            var humidityData = _azureService.GetBlobClient(deviceId, "humidity", date, _containerClient);
            if (humidityData.Exists())
                humidity = ArchiveService.GetArrayFromStream(humidityData.OpenRead());
            else
            {
                Stream str = AzureStorageService.GetHistoricalStream(deviceId, "humidity", _containerClient);
                using ZipArchive package = new(str, ZipArchiveMode.Read);
                ZipArchiveEntry? a = package.Entries.FirstOrDefault(e => e.Name == $"{date:yyyy-MM-dd}.csv");
                humidity = ArchiveService.GetArrayFromStream(a.Open());
            }

            return StatusCode((int)HttpStatusCode.OK, new { temperature, rain, humidity });
        }
    }
}