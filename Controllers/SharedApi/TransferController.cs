using Microsoft.AspNetCore.Mvc;
using chatWhatsappServer.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;
using chatWhatsappServer.Utils;
namespace chatWhatsappServer.Controllers.SharedApi 
{
    public class TransferScheme {
        public string from {get; set;}
        public string to {get; set;}
        public string server { get; set; }

    }

    [Route("api/[controller]")]
    public class TransferController: ControllerBase {

        private ContactQueries q;

        private IConfiguration conf;
        private SecUtils utils;
        private static readonly HttpClient client = new HttpClient();

        public TransferController(IConfiguration configuration)
        {
            conf = configuration;
            utils = new SecUtils(conf);
            q = new ContactQueries(conf);

        }

        [HttpPost]
        public async Task<IActionResult> transfer([FromBody] TransferScheme transfer) {

            Inbox contact = q.getContactByName(transfer.to);

            if(contact == null) {
                return BadRequest("Contact does not exists");
            }


              var values = new Dictionary<string, string>
                {
                    { "to", transfer.to },
                    { "from", transfer.from },
                    { "server",  contact.server}

                };

                var content = new FormUrlEncodedContent(values);
                string remoteServer = String.Format("http://{0}/api/transfer",  contact.server);
                var response = await client.PostAsync(remoteServer, content);

                var responseString = await response.Content.ReadAsStringAsync();

                return Ok();
        }
    }
    
    }