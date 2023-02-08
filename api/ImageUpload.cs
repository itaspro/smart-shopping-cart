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
using System.Security.Claims;
using System.Linq;
using Azure.Identity;
using Azure.Storage.Sas;
using Azure.Storage;

namespace smart.Function
{
  public static class ImageUpload
    {
        [FunctionName("ImageUpload")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = null )] HttpRequest req,ILogger log)
        {
            string accountName = Environment.GetEnvironmentVariable("AccountName");
            string accountKey  = Environment.GetEnvironmentVariable("AccountKey");
            string container = Environment.GetEnvironmentVariable("ContainerName");
            string blobServiceUrl = $"https://{accountName}.blob.core.windows.net";
            var key = new StorageSharedKeyCredential(accountName, accountKey);

            AccountSasBuilder sasBuilder = new AccountSasBuilder()
            {
                Services = AccountSasServices.Blobs,
                ResourceTypes = AccountSasResourceTypes.All,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
                Protocol = SasProtocol.Https
            };

            sasBuilder.SetPermissions(
                AccountSasPermissions.Read 
                | AccountSasPermissions.Write 
                | AccountSasPermissions.Create 
                | AccountSasPermissions.List 
                | AccountSasPermissions.Update);

            string sasToken = sasBuilder.ToSasQueryParameters(key).ToString();

            return new JsonResult(new { sasToken, container, blobServiceUrl });
        }
    }
}
