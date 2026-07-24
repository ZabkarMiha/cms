using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Core.Configurations;
using Microsoft.Extensions.Options;

namespace CMSApi.Business
{
    public interface IBlobBusinessLayer
    {
        Task CreateContainer(string containerName);
        Task<List<string>> ListContainers();
    }

    public class BlobBusinessLayer : IBlobBusinessLayer
    {
        private readonly ConnectionStrings _connectionOptions;
        private readonly BlobServiceClient _blobServiceClient;

        public BlobBusinessLayer(IOptions<ConnectionStrings> connectionOptions)
        {
            _connectionOptions = connectionOptions.Value;
            _blobServiceClient = new BlobServiceClient(_connectionOptions.AzureBlob);
        }

        public async Task CreateContainer(string containerName)
        {
            BlobContainerClient container = await _blobServiceClient.CreateBlobContainerAsync(
                containerName
            );
        }

        public async Task<List<string>> ListContainers()
        {
            var resultSegment = _blobServiceClient.GetBlobContainersAsync();
            List<string>? names = new List<string>();

            await foreach (BlobContainerItem containerPage in resultSegment)
            {
                names.Add(containerPage.Name);
            }

            return names;
        }
    }
}
