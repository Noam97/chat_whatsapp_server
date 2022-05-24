using Microsoft.AspNetCore.Mvc;
using chatWhatsappServer.DBModels;
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
                PostContact pc = new PostContact{id = invite.from, name = invite.from, server = invite.server, currentUser = invite.to};
                q.addNewContact(pc , invite.to);
                Response.StatusCode = 201;
                return Ok();
        }
    }
    
    }