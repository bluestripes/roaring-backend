using System;
using System.Collections.Generic;
using System.Text;

namespace Bluestripe_register.Configuration
{
    public class AzureSettings
    {
        public string ManagementApiBaseAddress { get; set; }
        public string SubscriptionId { get; set; }
        public string ApimServiceName { get; set; }
        public string ApimResourceGroupName { get; set; }
    }
}
