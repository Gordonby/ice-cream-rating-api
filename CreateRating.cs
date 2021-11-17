using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
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
            //log.LogInformation("GOT HERE 1");
            
            // Get json values

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            //Rating rating =new Rating();
            var rating = JsonConvert.DeserializeObject<Rating>(body);

            // Validate both userId and productId by calling the existing API endpoints. You can find a user id to test with from the sample payload above

            // Validate user - https://serverlessohapi.azurewebsites.net/api/GetUser?userId=[USER ID]]
            
            HttpClient newClient = new HttpClient();
            HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("https://serverlessohapi.azurewebsites.net/api/GetUser?userId={0}", rating.userId));
            HttpResponseMessage response = await newClient.SendAsync(newRequest);
            

            //log.LogInformation("GOT HERE 2");
            string responseString = await response.Content.ReadAsStringAsync();

            if (responseString.ToUpper() == "USER DOES NOT EXIST")
                return new NotFoundResult();
            
            //log.LogInformation(responseString);
            //log.LogInformation("GOT HERE 3");

            // Add a property called id with a GUID value
            rating.id = Guid.NewGuid().ToString();

            // Add a property called timestamp with the current UTC date time
            rating.timestamp = DateTime.UtcNow.ToString();

            // Validate that the rating field is an integer from 0 to 5
            int result;
            if (int.TryParse(rating.rating, out result))
            {
                if (result < 0 || result > 5)
                {
                    return new NotFoundResult();
                }
            }
            else
                return new NotFoundResult();

            // Use a data service to store the ratings information to the backend
            await documentsOut.AddAsync(rating);

            // Return the entire review JSON payload with the newly created id and timestamp
            return new OkObjectResult(rating);
        }
    }
}
