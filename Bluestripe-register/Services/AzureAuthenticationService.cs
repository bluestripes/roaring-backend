using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Threading.Tasks;

namespace Bluestripe_register.Services
{
    public class AzureAuthenticationService
    {
        public async Task<string> GetToken()
        {
            try
            {
                var tokenProvider = new AzureServiceTokenProvider();
                var accesstoken = await tokenProvider.GetAccessTokenAsync("https://management.azure.com");
                return accesstoken;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
