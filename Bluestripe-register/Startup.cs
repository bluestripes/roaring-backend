using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Bluestripe_register.Services;
using Bluestripe_register.Configuration;
using Microsoft.Azure.Services.AppAuthentication;

[assembly: FunctionsStartup(typeof(Bluestripe_register.Startup))]
namespace Bluestripe_register
{
    public class Startup : FunctionsStartup
    {
        public IConfiguration Configuration;
        public override void Configure(IFunctionsHostBuilder builder)
        {
            SetupAppSettings(builder.Services);
            builder.Services.AddSingleton<AzureAuthenticationService>();
            SetupHttpClients(builder.Services, Configuration);
        }

        public void SetupHttpClients(IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient<ApimService>();
            services.AddHttpClient<RoaringService>();
        }

        public void SetupAppSettings(IServiceCollection services)
        {
            //When running locally, create a local.settings.json file and we will read from there. Deployed, we read from app settings in Azure
            var executionContextOptions = services.BuildServiceProvider().GetService<IOptions<ExecutionContextOptions>>().Value;
            var appDirectory = executionContextOptions.AppDirectory;
            var appConfig = new ConfigurationBuilder()
            .SetBasePath(appDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

            services.Configure<AzureSettings>(option =>
            {
                option.ManagementApiBaseAddress = appConfig.GetSection("AzureSettings:ManagementApiBaseAddress").Value;
                option.SubscriptionId = appConfig.GetSection("AzureSettings:SubscriptionId").Value;
                option.ApimResourceGroupName = appConfig.GetSection("AzureSettings:ApimResourceGroupName").Value;
                option.ApimServiceName = appConfig.GetSection("AzureSettings:ApimServiceName").Value;
            });

            services.Configure<RoaringSettings>(option =>
            {
                option.BaseAddress = appConfig.GetSection("RoaringSettings:BaseAddress").Value;
                option.TokenUri = appConfig.GetSection("RoaringSettings:TokenUri").Value;
            });

            Configuration = appConfig;
        }
    }
}
