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

namespace Lauka.pi
{
    public static class TextPost
    {
        [FunctionName("TextPost")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "text")] HttpRequest req)
        {
            string request = await new StreamReader(req.Body).ReadToEndAsync();

            TextRequest text;
            try
            {
                text = JsonConvert.DeserializeObject<TextRequest>(request);
            }
            catch
            {
                return new BadRequestResult();
            }

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject( new Dictionary<string, string>() { {"status", "success" } } ),
                ContentType = "application/json",
                StatusCode = 200
            };
        }
    }
}
