using Azure.Storage.Blobs;
using NexerInsight.Models;
using System.IO.Compression;

namespace NexerInsight.Services
{
    public class AzureStorageService
    {
        private readonly string _connectionString;

        public AzureStorageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage");
        }

        internal static IEnumerable<SensorReading> GetSensorData(string deviceId, Enums.SensorType sensorType, DateTime date, BlobContainerClient containerClient)
        {
            IEnumerable<SensorReading> sensorData = new List<SensorReading>();
            var tempData = containerClient.GetBlobClient($"{deviceId}/{sensorType}/{date:yyyy-MM-dd}.csv");
            if (tempData.Exists())
                sensorData = ArchiveService.GetArrayFromStream(tempData.OpenRead());
            else
            {
                Stream str = GetHistoricalStream(deviceId, sensorType.ToString(), containerClient);
                using ZipArchive package = new(str, ZipArchiveMode.Read);
                ZipArchiveEntry? a = package.Entries.FirstOrDefault(e => e.Name == $"{date:yyyy-MM-dd}.csv");
                if (a != null)
                    sensorData = ArchiveService.GetArrayFromStream(a.Open());
            }
            return sensorData;
        }

        internal BlobContainerClient GetBlobContainerClient(string blobName) => new BlobServiceClient(_connectionString).GetBlobContainerClient(blobName);

        internal static Stream GetHistoricalStream(string deviceId, string sensorType, BlobContainerClient containerClient)
        {
            var blobClient = containerClient.GetBlobClient($"{deviceId}/{sensorType}/historical.zip");
            return blobClient.OpenRead();
        }
    }
}
