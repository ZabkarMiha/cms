using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Core.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Core.Services
{
    public interface IFileService
    {
        Task<string> UploadProfilePicture(IFormFile file, Guid userId);
        Task<string> UploadCarPicture(IFormFile file, Guid carId);
        Task DeleteProfilePicture(string url);
        Task DeleteCarPicture(string url);
        Task<Uri> RetrieveProfilePictureUrl(string url);
        Task<Uri> RetrieveCarPictureUrl(string url);
    }

    public class FileService : IFileService
    {
        private readonly ConnectionStrings _connectionOptions;
        private readonly AzureAccount _azureOptions;
        private readonly BlobServiceClient _blobServiceClient;

        public FileService(
            IOptions<ConnectionStrings> connectionOptions,
            IOptions<AzureAccount> azureOptions
        )
        {
            _connectionOptions = connectionOptions.Value;
            _azureOptions = azureOptions.Value;
            _blobServiceClient = new BlobServiceClient(_connectionOptions.AzureBlob);
        }

        public async Task<string> UploadProfilePicture(IFormFile file, Guid userId)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(
                "profile-pictures"
            );

            return await Upload(file, blobContainerClient, userId);
        }

        public async Task<string> UploadCarPicture(IFormFile file, Guid carId)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(
                "car-pictures"
            );

            return await Upload(file, blobContainerClient, carId);
        }

        public async Task DeleteProfilePicture(string url)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(
                "profile-pictures"
            );

            await Delete(blobContainerClient, url);
        }

        public async Task DeleteCarPicture(string url)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(
                "car-pictures"
            );

            await Delete(blobContainerClient, url);
        }

        public async Task<Uri> RetrieveProfilePictureUrl(string url)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(
                "profile-pictures"
            );

            return await Retrieve(blobContainerClient, url);
        }

        public async Task<Uri> RetrieveCarPictureUrl(string url)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(
                "car-pictures"
            );

            return await Retrieve(blobContainerClient, url);
        }

        private async Task<string> Upload(
            IFormFile file,
            BlobContainerClient blobContainerClient,
            Guid id
        )
        {
            string extension = Path.GetExtension(file.FileName).ToLower();

            BlobClient blobClient = blobContainerClient.GetBlobClient(id + extension);

            var blobHttpHeader = new BlobHttpHeaders();
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    blobHttpHeader.ContentType = "image/jpeg";
                    break;
                case ".png":
                    blobHttpHeader.ContentType = "image/png";
                    break;
                case ".gif":
                    blobHttpHeader.ContentType = "image/gif";
                    break;
                default:
                    break;
            }

            await blobClient.UploadAsync(file.OpenReadStream(), blobHttpHeader);

            return blobClient.Uri.ToString();
        }

        private async Task Delete(BlobContainerClient blobContainerClient, string url)
        {
            Uri blobUri = new Uri(url);
            BlobClient blobClient = blobContainerClient.GetBlobClient(
                Path.GetFileName(blobUri.LocalPath)
            );

            await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        private Task<Uri> Retrieve(BlobContainerClient blobContainerClient, string url)
        {
            Uri blobUri = new Uri(url);
            BlobClient blobClient = blobContainerClient.GetBlobClient(
                Path.GetFileName(blobUri.LocalPath)
            );

            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobContainerClient.Name,
                BlobName = blobClient.Name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            string sasToken = sasBuilder
                .ToSasQueryParameters(
                    new StorageSharedKeyCredential(
                        _azureOptions.AccountName,
                        _azureOptions.AccountKey
                    )
                )
                .ToString();

            return Task.FromResult(new Uri($"{url}?{sasToken}"));
        }
    }
}
