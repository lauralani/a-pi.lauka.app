using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Lauka.pi.Classes;
using System.Collections.Generic;
using RestSharp;

namespace Lauka.pi
{
    public static class TextPost
    {
        [FunctionName("TextPost")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "text")] HttpRequest req)
        {
            string request = await new StreamReader(req.Body).ReadToEndAsync();

            int fontsize = int.Parse(Environment.GetEnvironmentVariable("APP_BACKEND_DEFAULT_FONTSIZE"));

            TextRequest text;
            try
            {
                text = JsonConvert.DeserializeObject<TextRequest>(request);
                Console.WriteLine(text.Text);
            }
            catch
            {
                return new BadRequestObjectResult("bad body");
            }

            Dictionary<string, object> restbody = new Dictionary<string, object>()
            {
                {"text", text.Text},
                {"fontsize", fontsize}
            };

            RestClient restclient = new RestClient("https://a-pi-backend.lauka.app/text");
            restclient.Timeout = -1;
            RestRequest restrequest = new RestRequest(Method.POST);
            restrequest.AddHeader("x-api-key", Environment.GetEnvironmentVariable("APP_BACKEND_APIKEY"));
            restrequest.AddHeader("Content-Type", "application/json");
            restrequest.AddParameter("application/json", JsonConvert.SerializeObject(restbody), ParameterType.RequestBody);
            IRestResponse restresponse = restclient.Execute(restrequest);

            if (restresponse.IsSuccessful)
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(new Dictionary<string, string>() { { "status", "success" } }),
                    ContentType = "application/json",
                    StatusCode = 200
                };
            else
                return new BadRequestObjectResult("something went wrong");
        }
    }
}
