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

        /// <summary>
        /// Get the data from azure storage
        /// </summary>
        /// <param name="deviceId">Device id to search</param>
        /// <param name="sensorType">Sensor type to search</param>
        /// <param name="date">Date to search</param>
        /// <param name="containerClient">BlobContainerClient of azure storage</param>
        /// <returns></returns>
        internal static IEnumerable<SensorReading> GetSensorData(string deviceId, Enums.SensorType sensorType, DateTime date, BlobContainerClient containerClient)
        {
            IEnumerable<SensorReading> sensorData = new List<SensorReading>();

            var tempData = containerClient.GetBlobClient($"{deviceId}/{sensorType}/{date:yyyy-MM-dd}.csv");
            if (tempData.Exists())
                return ArchiveService.GetArrayFromStream(tempData.OpenRead());

            var blobClient = containerClient.GetBlobClient($"{deviceId}/{sensorType}/historical.zip");
            if (blobClient.Exists())
            {
                using Stream str = blobClient.OpenRead();

                using ZipArchive package = new(str, ZipArchiveMode.Read);
                ZipArchiveEntry? zipArchive = package.Entries.FirstOrDefault(e => e.Name == $"{date:yyyy-MM-dd}.csv");
                if (zipArchive != null)
                    sensorData = ArchiveService.GetArrayFromStream(zipArchive.Open());
            }
            return sensorData;
        }

        internal BlobContainerClient GetBlobContainerClient(string blobName) => new BlobServiceClient(_connectionString).GetBlobContainerClient(blobName);
    }
}
