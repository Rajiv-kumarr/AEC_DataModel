using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class AecDataModelController : ApiController
    {
        //public async Task<Dictionary<string, string>> GetAsync()
        //{
        //    Dictionary<string, string> hubDetails = new Dictionary<string, string>();
        //    string authCode = await FusionOAuth.OAuth.GenerateAuthCode();
        //    string token3legged = await FusionOAuth.OAuth.GenerateTokenCode3Legged(authCode);
        //    hubDetails = await HubInformation.HubInfo.GetHubDetails(token3legged);

        //    Console.WriteLine("hubDetails: " + hubDetails);

        //    return hubDetails;
        //}
        public Task<string> Get()
        {
            // Simulate an asynchronous operation
            return Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ => "This is asynchronous data");
        }
    }
}
