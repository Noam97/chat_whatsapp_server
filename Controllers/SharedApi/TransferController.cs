using Microsoft.AspNetCore.Mvc;
using chatWhatsappServer.DBModels;
using chatWhatsappServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using chatWhatsappServer.Utils;
namespace chatWhatsappServer.Controllers.SharedApi 
{
    public class TransferScheme {
        public string from {get; set;}
        public string to {get; set;}
        public string content { get; set; }

    }

    [Route("api/[controller]")]
    public class TransferController: ControllerBase {

        private ContactQueries q;

        private IConfiguration conf;
        private SecUtils utils;

        private IHubContext<LogicHub> _hub;
        private static readonly HttpClient client = new HttpClient();

        public TransferController(IConfiguration configuration, IHubContext<LogicHub> hub)
        {
            conf = configuration;
            utils = new SecUtils(conf);
            q = new ContactQueries(conf);
            _hub = hub;

        }

        [HttpPost]
        public async Task<IActionResult> transfer([FromBody] TransferScheme transfer) {
        using ( var db = new EFContext(conf) ) 
        {
            InboxParticipants contactOne = db.InboxParticipants.Where(u=> u.UserId == transfer.to).FirstOrDefault();

            Inbox contactOneInbox = q.getContactByName(transfer.from, contactOne.inboxUID);

            Messages msg = new Messages{inboxUID = contactOne.inboxUID,
            UserId = transfer.from, messageType = "text", content = transfer.content, created = DateTime.UtcNow.ToString(), sent = true};
            db.Messages.Add(msg);
            db.SaveChanges();

            PostContact pcOne = new PostContact{name = transfer.from, server = contactOneInbox.server};

            q.updateContact(contactOneInbox, pcOne, msg);

            _hub.Clients.All.SendAsync("ReceivedMessage", transfer.from, transfer.to, contactOne.inboxUID, transfer.content);

            Response.StatusCode = 201;
            return Ok();
        } 
    }
        
    }
    
    }