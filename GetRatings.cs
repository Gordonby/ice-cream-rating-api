using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace icecream.Rating
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "ratings/users/{userId}")] 
                HttpRequest req,
                [CosmosDB(
                    databaseName: "icecream", 
                    collectionName: "ratings", 
                    ConnectionStringSetting = "COSMOS_CONNECTION_STRING",
                    SqlQuery = "SELECT * FROM ratings r where r.userId={userId}")] IEnumerable<Rating> ratings,
                ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (ratings is null)
            {
                return new NotFoundResult();
            }
 
            return new OkObjectResult(ratings); 

            /*
        

            string userId = req.Query["userId"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userId = userId ?? data?.userId;

            // Connect to Cosmos DB
            */

            /*
            string responseMessage = string.IsNullOrEmpty(userId)
                ? "This HTTP triggered function executed successfully. Pass a userId in the query string or in the request body for a personalized response." : "";

            return new OkObjectResult(responseMessage);
            */
        }
    }

    public class Rating
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string productId { get; set; }
        public string timestamp { get; set; }
        public string locationName { get; set; }
        public string rating { get; set; }
        public string userNotes { get; set; }
    }
}
