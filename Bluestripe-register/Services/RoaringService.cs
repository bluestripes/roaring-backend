using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Bluestripe_register.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Bluestripe_register.Models;

namespace Bluestripe_register.Services
{

    public class RoaringService
    {
        private readonly HttpClient _httpClient;
        private readonly RoaringSettings _roaringSettings;

        public RoaringService(HttpClient HttpClient, IOptions<RoaringSettings> roaringSettings)
        {
            _roaringSettings = roaringSettings.Value;
            _httpClient = HttpClient;
        }

        public async Task<Token> GetToken(string base64EncodedClientCredentials)
        {
            try
            {
                var dict = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" }
                };
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, _roaringSettings.TokenUri)
                {
                    Content = new FormUrlEncodedContent(dict)
                };
                req.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedClientCredentials);
                var response = await _httpClient.SendAsync(req);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Token>(content);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> ValidateClientCredentials(string base64EncodedClientCredentials)
        {
            try
            {
                var dict = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" }
                };
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, _roaringSettings.TokenUri) 
                { 
                    Content = new FormUrlEncodedContent(dict) 
                };
                req.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedClientCredentials);
                var response = await _httpClient.SendAsync(req);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
