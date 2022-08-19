using Azure.Storage.Blobs;

namespace NexerInsight.Services
{
    public class AzureStorageService
    {
        private readonly string _connectionString;

        public AzureStorageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage");
        }

        internal BlobContainerClient GetBlobContainerClient(string blobName) => new BlobServiceClient(_connectionString).GetBlobContainerClient(blobName);

        internal static Stream GetStreamFromBlobClient(BlobClient blobClient) => blobClient.OpenRead();

        internal static Stream GetHistoricalStream(string deviceId, string sensorType, BlobContainerClient containerClient)
        {
            var blobClient = containerClient.GetBlobClient($"{deviceId}/{sensorType}/historical.zip");
            return GetStreamFromBlobClient(blobClient);
        }
    }
}
