using Bluestripe_register.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bluestripe_register.Services
{
    public class ApimService
    {
        private readonly HttpClient _httpClient;
        private readonly AzureSettings _azureSettings;
        private readonly AzureAuthenticationService _azureAuthenticationService;
        public string AccessToken;

        public ApimService(HttpClient httpClient, IOptions<AzureSettings> azureSettings, AzureAuthenticationService authenticationService)
        {
            _httpClient = httpClient;
            _azureSettings = azureSettings.Value;
            _azureAuthenticationService = authenticationService;
        }

        public async Task<string> CreateSubscription(string email, string apiName)
        {
            try
            {
                AccessToken = await _azureAuthenticationService.GetToken();

                string domainName = email.Split('@')[1].Split('.')[0];
                var uri = $"/subscriptions/{_azureSettings.SubscriptionId}" +
                    $"/resourceGroups/{_azureSettings.ApimResourceGroupName}/providers/Microsoft.ApiManagement" +
                    $"/service/{_azureSettings.ApimServiceName}" +
                    $"/subscriptions/{domainName}?api-version=2019-12-01";

                var fullUrl = "https://management.azure.com" + uri;

                var req = new HttpRequestMessage(HttpMethod.Put, fullUrl);
                req.Headers.Add("Authorization", $"Bearer {AccessToken}");
                req.Content = new StringContent(@"{""properties"": " +
                    $"{{\"ownerId\": \"/subscriptions/{_azureSettings.SubscriptionId}/resourceGroups" +
                                                    $"/{_azureSettings.ApimResourceGroupName}/providers/Microsoft.ApiManagement" +
                                                    $"/service/{_azureSettings.ApimServiceName}/users/{apiName}\", " +
                    $"\"displayName\": {domainName}," +
                    $"\"scope\" : \"/apis/{apiName}\"}}",
                    Encoding.UTF8, "application/json");

                var result = await _httpClient.SendAsync(req);
                result.EnsureSuccessStatusCode();
                var content = await result.Content.ReadAsStringAsync();
                return content;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
