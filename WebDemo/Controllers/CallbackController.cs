using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebDemo.Controllers
{
    [RoutePrefix("api/Callback")]
    public class CallbackController : ApiController
    {
        [Route("Demo")]
        public Dictionary<string, string> Get()
        {
            var result = new Dictionary<string, string>()
            {
                { "001", "Banana" },
                { "002", "Apple" }
            };
            return result;
        }

        public async Task<dynamic> Post()
        {
            try
            {
                var contentString = await Request.Content.ReadAsStringAsync();

                dynamic contentObj = JsonConvert.DeserializeObject(contentString);
                var result = contentObj.result[0];

                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
