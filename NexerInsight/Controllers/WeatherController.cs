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
            IEnumerable<SensorReading> sensorData = AzureStorageService
                .GetSensorData(deviceId, (SensorType)Enum.Parse(typeof(SensorType), sensorType), date, _containerClient);
            return StatusCode((int)HttpStatusCode.OK, sensorData);
        }

        [HttpGet("GetDataForDevice")]
        public IActionResult GetDataForDevice(DateTime date, string deviceId)
        {
            IEnumerable<SensorReading> temperature = AzureStorageService.GetSensorData(deviceId, SensorType.temperature, date, _containerClient);
            IEnumerable<SensorReading> rainfall = AzureStorageService.GetSensorData(deviceId, SensorType.rainfall, date, _containerClient);
            IEnumerable<SensorReading> humidity = AzureStorageService.GetSensorData(deviceId, SensorType.humidity, date, _containerClient);
            return StatusCode((int)HttpStatusCode.OK, new { temperature, rainfall, humidity });
        }
    }
}