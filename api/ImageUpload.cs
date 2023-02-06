using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace smart.Function
{
  public static class ImageUpload
    {
        [FunctionName("ImageUpload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");
            var blobClient = new BlobContainerClient(Connection, containerName);
            var blob = blobClient.GetBlobClient("testfile.json");
            var resp = await blob.UploadAsync(req.Body);
            
            string responseMessage = $"image upload";
            log.LogInformation(responseMessage);

            return new OkObjectResult(responseMessage);
        }
    }
}
