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
                Stream str = blobClient.OpenRead();

                using ZipArchive package = new(str, ZipArchiveMode.Read);
                ZipArchiveEntry? a = package.Entries.FirstOrDefault(e => e.Name == $"{date:yyyy-MM-dd}.csv");
                if (a != null)
                    sensorData = ArchiveService.GetArrayFromStream(a.Open());
            }
            return sensorData;
        }

        internal BlobContainerClient GetBlobContainerClient(string blobName) => new BlobServiceClient(_connectionString).GetBlobContainerClient(blobName);

        /// <summary>
        /// Get the stream from the zipped history.
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <param name="sensorType">Sensor type</param>
        /// <param name="containerClient">BlobContainerClient of azure storage</param>
        /// <returns></returns>
        internal static Stream GetHistoricalStream(string deviceId, string sensorType, BlobContainerClient containerClient)
        {
            var blobClient = containerClient.GetBlobClient($"{deviceId}/{sensorType}/historical.zip");
            return blobClient.OpenRead();
        }
    }
}
