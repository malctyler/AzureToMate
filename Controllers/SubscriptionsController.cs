using Microsoft.AspNetCore.Mvc;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace test_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionInfo>>> GetSubscriptions()
        {
            try
            {
                var credential = new DefaultAzureCredential();
                var armClient = new ArmClient(credential);
                var subscriptions = armClient.GetSubscriptions();

                var subscriptionList = new List<SubscriptionInfo>();

                await foreach (var subscription in subscriptions)
                {
                    subscriptionList.Add(new SubscriptionInfo
                    {
                        Id = subscription.Data.SubscriptionId,
                        Name = subscription.Data.DisplayName
                    });
                }

                return Ok(subscriptionList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class SubscriptionInfo
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}