using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.Collections.Generic;
using Azure.Storage.Blobs.Models;

namespace smart.Function
{
  public static class ImageUpload
    {
        [FunctionName("ImageUpload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string Connection = Environment.GetEnvironmentVariable("AzBlobStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");
            var label = req.Headers["label"].ToString();
            var argument = req.Headers["argument"];
            var metaData = new Dictionary<string, string>() { {"label", label}};
            var blobClient = new BlobContainerClient(Connection, containerName);
            var blob = blobClient.GetBlobClient($"{label}__{Path.GetRandomFileName()}");
            BlobUploadOptions options = new BlobUploadOptions {
                Metadata = metaData
            };
            var resp = await blob.UploadAsync(req.Body, options);
            var http = resp.GetRawResponse();
            if (http.Status < 300) {
                 return new OkObjectResult("uploaded");
            } else {
                using(var read = new StreamReader(http.ContentStream)) {
                    var msg = read.ReadToEnd();
                    log.LogInformation(msg);
                    var result = new ObjectResult(msg);
                    result.StatusCode = http.Status;
                    return result;
                }
            }
        }
    }
}
