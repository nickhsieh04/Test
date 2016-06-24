using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            try
            {
                IntoDB("Test @ " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                var result = new Dictionary<string, string>()
            {
                { "001", "Banana" },
                { "002", "Apple" }
            };
                return result;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>()
                {
                    { "error", ex.ToString() }
                };
            }
        }

        public async Task<HttpResponseMessage> Post()
        {
            try
            {
                IntoDB("Post ..");

                var contentString = await Request.Content.ReadAsStringAsync();

                dynamic contentObj = JsonConvert.DeserializeObject(contentString);

                IntoDB(Convert.ToString(contentObj));

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
                IntoDB(ex.ToString());
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        private void IntoDB(string inStr)
        {
            string connectionstring = "Server=46ee7849-06f9-4996-9e33-a62f002a09fa.sqlserver.sequelizer.com;Database=db46ee784906f949969e33a62f002a09fa;User ID=qaryukkuimzqcwoc;Password=s6qpoVU8RWLjY8FPGbFpWpfPsxNDCrxEfUt6Z8y63k2KPXjqys4qFHWcZJ6YgpDz;";

            connectionstring = "Data Source=46ee7849-06f9-4996-9e33-a62f002a09fa.sqlserver.sequelizer.com;" +
                "Initial Catalog=db46ee784906f949969e33a62f002a09fa;" +
                "User id=qaryukkuimzqcwoc;" +
                "Password=s6qpoVU8RWLjY8FPGbFpWpfPsxNDCrxEfUt6Z8y63k2KPXjqys4qFHWcZJ6YgpDz;";

            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();

                string SQL = "insert into [debug] (event) values (@event);";

                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.Parameters.Add(new SqlParameter("@event", inStr));
                cmd.ExecuteNonQuery();
            }
        }
    }
}
