using Microsoft.AspNetCore.Mvc;
using chatWhatsappServer.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;
using chatWhatsappServer.Utils;
namespace chatWhatsappServer.Controllers.SharedApi 
{
    public class InviteScheme {
        public string from {get; set;}
        public string to {get; set;}
        public string server { get; set; }

    }

    [Route("api/[controller]")]
    public class invitationsController: ControllerBase {

        private ContactQueries q;

        private IConfiguration conf;
        private SecUtils utils;
        private static readonly HttpClient client = new HttpClient();

        public invitationsController(IConfiguration configuration)
        {
            conf = configuration;
            utils = new SecUtils(conf);
            q = new ContactQueries(conf);

        }

        [HttpPost]
        public async Task<IActionResult> invite([FromBody] InviteScheme invite) {
              var values = new Dictionary<string, string>
                {
                    { "to", invite.to },
                    { "from", invite.from },
                    { "server", invite.server }

                };

                var content = new FormUrlEncodedContent(values);
                string remoteServer = String.Format("http://{0}/api/invitations", invite.server);
                var response = await client.PostAsync(remoteServer, content);

                var responseString = await response.Content.ReadAsStringAsync();

                return Ok();
        }
    }
    
    }