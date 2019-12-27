﻿using HOW.AspNetCore.Services.Interfaces;
using HOW.AspNetCore.Services.Options;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HOW.AspNetCore.Services.Storage
{
    public class AzureBlobService : IStorageService
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly AzureBlobServiceOptions _options;

        public AzureBlobService(IOptionsMonitor<AzureBlobServiceOptions> options)
        {
            _options = options.CurrentValue;
            _storageAccount = CloudStorageAccount.Parse(_options.ConnectionString);
        }

        public async Task<Uri> SaveFileAsync(Stream fileStream, string fileName)
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_options.TargetContainer.ToLower());

            var options = new BlobRequestOptions();

            // Create the container if it doesn't already exist. Set Public access to container and blobs
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, options, new OperationContext());

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromStreamAsync(fileStream, fileStream.Length);

            return blockBlob.Uri;
        }

        public async Task<Uri> SaveFileAsync(byte[] fileContents, string fileName)
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_options.TargetContainer.ToLower());

            // Create the container if it doesn't already exist.
            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromByteArrayAsync(fileContents, 0, fileContents.Length);

            return blockBlob.Uri;
        }

        public async Task RemoveFileAsync(string blobUrl)
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_options.TargetContainer.ToLower());

            Uri fileLocation = new Uri(blobUrl);
            string fileToDelete = Path.GetFileName(fileLocation.LocalPath);

            await container.GetBlobReference(fileToDelete).DeleteIfExistsAsync();
        }

        public async Task<Stream> GetFileAsStreamAsync(string fileName)
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_options.TargetContainer.ToLower());

            MemoryStream outputStream = new MemoryStream();
            await container.GetBlobReference(fileName).DownloadToStreamAsync(outputStream);
            return outputStream;
        }

        public async Task<byte[]> GetFileAsByteArrayAsync(string fileName)
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_options.TargetContainer.ToLower());

            byte[] fileBytes = new byte[4096];

            await container.GetBlobReference(fileName).DownloadToByteArrayAsync(fileBytes, 0);
            return fileBytes;
        }

        private async Task<string> GenerateSASTokenAsync(Uri docUri)
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_options.TargetContainer.ToLower());

            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Off
            };

            permissions.SharedAccessPolicies.Clear();
            permissions.SharedAccessPolicies.Add("twominutepolicy", new SharedAccessBlobPolicy());
            await container.SetPermissionsAsync(permissions);

            SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(2),
                Permissions = SharedAccessBlobPermissions.Read
            };

            CloudBlockBlob blob = container.GetBlockBlobReference(docUri.ToString());

            return blob.GetSharedAccessSignature(sharedPolicy, "twominutepolicy");
        }
    }
}
