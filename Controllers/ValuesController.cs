using HubInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public string Get()
        {
            return "hello world!!!";
        }

        //public async Task<Dictionary<string, string>> GetAsync()
        //{
        //    Dictionary<string, string> hubDetails = new Dictionary<string, string>();
        //    Dictionary<string, string> projectDetails = new Dictionary<string, string>();

        //    string authCode = await FusionOAuth.OAuth.GenerateAuthCode();
        //    string token3legged = await FusionOAuth.OAuth.GenerateTokenCode3Legged(authCode);
        //    hubDetails = await HubInformation.HubInfo.GetHubDetails(token3legged);

        //    //foreach(var hubDetail in hubDetails)
        //    //{
        //    //    string hubId = hubDetail.Key;
        //    //    string hubName = hubDetail.Value;

        //    //    projectDetails = await HubInformation.HubInfo.GetProjectDetails(token3legged, hubId);
        //    //}

        //    return hubDetails;
        //}

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
