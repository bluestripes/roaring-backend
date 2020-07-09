using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Bluestripe_register.Services;
using Bluestripe_register.Models;

namespace Bluestripe_register
{
    public class Register
    {
        private readonly ApimService _apimService;
        private readonly RoaringService _roaringService;
        public Register(ApimService apimService, RoaringService roaringService)
        {
            _apimService = apimService;
            _roaringService = roaringService;
        }
        [FunctionName("Register")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            req.Headers.TryGetValue("Authorization", out var encodedHeader);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            RegisterModel body = JsonConvert.DeserializeObject<RegisterModel>(requestBody);
            var apiName = body.ApiName.ToLower();
            bool isSuccessfullyAuthenticated;
            switch (apiName)
            {
                case "roaring":
                    isSuccessfullyAuthenticated = await _roaringService.ValidateClientCredentials(encodedHeader);
                    break;
                default:
                    return new BadRequestObjectResult(new NotImplementedException($"There is no implementation for api: {body.ApiName}"));
            }

            if(isSuccessfullyAuthenticated)
            {
                var result = await _apimService.CreateSubscription(body.Email, apiName);
                return new OkObjectResult(result);
            }

            return new BadRequestResult();
        }
    }
}
