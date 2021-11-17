using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace icecream.Rating
{
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "icecream",
            collectionName: "ratings",
            ConnectionStringSetting = "COSMOS_CONNECTION_STRING")] IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var rating = JsonConvert.DeserializeObject(body) as Rating;

            // Add a JSON document to the output container.
            await documentsOut.AddAsync(rating);

            // all done notify caller with status code 200
            return new OkObjectResult("adding succeeded");
        }
    }
}
