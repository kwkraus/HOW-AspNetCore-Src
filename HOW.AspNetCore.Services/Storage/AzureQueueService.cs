﻿using HOW.AspNetCore.Services.Options;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace HOW.AspNetCore.Services.Storage
{
    public class AzureQueueService
    {
        // Parse the connection string and return a reference to the storage account.
        private readonly CloudStorageAccount _storageAccount;
        private readonly AzureQueueServiceOptions _options;

        public AzureQueueService(IOptionsMonitor<AzureQueueServiceOptions> options)
        {
            _options = options.CurrentValue;
            _storageAccount = CloudStorageAccount.Parse(_options.ConnectionString);
        }

        public async Task SendProcessingMessageAsync<T>(T blobInfo) where T : class
        {
            CloudQueueClient queueClient = _storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(_options.ProcessingQueue);
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(blobInfo));

            await queue.CreateIfNotExistsAsync();
            await queue.AddMessageAsync(queueMessage);
        }

        public async Task<CloudQueueMessage> PeekAtProcessingMessageAsync()
        {
            CloudQueueClient queueClient = _storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(_options.ProcessingQueue);
            var message = await queue.PeekMessageAsync();

            return message;
        }

        public async Task<bool> ClearProcessingQueueAsync()
        {
            CloudQueueClient queueClient = _storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(_options.ProcessingQueue);

            try
            {
                await queue.ClearAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
