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

        public async Task<HttpResponseMessage> Post()
        {
            try
            {
                var contentString = await Request.Content.ReadAsStringAsync();

                dynamic contentObj = JsonConvert.DeserializeObject(contentString);
                var result = contentObj.result[0];

                var client = new HttpClient();
                
                string ChannelID = "1471588998";
                string ChannelSecret = "643ce88c48e6c6e9e30fb3fe29eca46e";
                string Mid = "u06604510a1ca03d518d8295f7b9d799c";

                client.DefaultRequestHeaders
                    .Add("X-Line-ChannelID", ChannelID);
                client.DefaultRequestHeaders
                    .Add("X-Line-ChannelSecret", ChannelSecret);
                client.DefaultRequestHeaders
                    .Add("X-Line-Trusted-User-With-ACL", Mid);


                var res = await client.PostAsJsonAsync("https://trialbot-api.line.me/v1/events",
                    new
                    {
                        to = new[] { result.content.from },
                        toChannel = "1383378250",
                        eventType = "138311608800106203",
                        content = new
                        {
                            contentType = 1,
                            toType = 1,
                            text = $"安安「{result.content.text}」你好"
                        }
                    });

                System.Diagnostics.Debug.WriteLine(await res.Content.ReadAsStringAsync());
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
