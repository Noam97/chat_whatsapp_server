using Microsoft.AspNetCore.Mvc;
using chatWhatsappServer.DBModels;
using chatWhatsappServer.Models;
using System.Text.Json;

namespace chatWhatsappServer.Controllers; 

public class ChatController : Controller
{
    private IConfiguration conf;
    private ContactQueries q;

    public ChatController(IConfiguration configuration)
    {
        conf = configuration;
        q = new ContactQueries(conf);

    }
 
    // show chat
    public ActionResult Index(UserIdModel person)
    {
        string id = person.Id;
        User user = q.getUser(id);
        ViewBag.currentUser = user;
        return View();
    }

    // get contacts
    [HttpPost]
    public ContentResult GetContacts([FromBody] UserIdModel person) {
        string id = person.Id;
        List<Inbox> contacts = q.getContactList(id);
        return Content(JsonSerializer.Serialize(contacts), "application/json");
    }


    [HttpPost]
    public async Task<IActionResult> createContact([FromBody] PostContact newContact) {
        string currentUserId = newContact.currentUser;
        q.addNewContact(newContact, currentUserId);

        if (newContact.server != "") {
            var values = new Dictionary<string, string>
            {
                { "to", newContact.id },
                { "from", currentUserId },
                { "server", Request.Host.ToString() }

            };
            HttpClient client = new HttpClient();
            var content = new FormUrlEncodedContent(values);
            string remoteServer = String.Format("http://{0}/api/invitations", newContact.server);
            var response = await client.PostAsync(remoteServer, content);
            var responseString = await response.Content.ReadAsStringAsync();
        }

        return Ok();
        }

    // get messages
    [HttpPost]
    public ContentResult GetMessages([FromBody] PostContact contact) {
        string currentUserId = contact.currentUser;
        List<Messages> msgs = q.getChat(currentUserId, contact.id);
        return Content(JsonSerializer.Serialize(msgs), "application/json");
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewMessage([FromBody] PostMessage newMessage) {

        using ( var db = new EFContext(conf) ) {

            if (newMessage.content == "") {
                return Ok();
            }

        Messages msg = new Messages{inboxUID = newMessage.inboxUID,
        UserId = newMessage.id, sender = newMessage.sender, messageType = newMessage.messageType, content = newMessage.content, created = DateTime.UtcNow.ToString(), sent = newMessage.sent};
        db.Messages.Add(msg);
        db.SaveChanges();
        
        Inbox contactOne = q.getContactByName(newMessage.id, newMessage.inboxUID);

        if(contactOne.server != "") {
            var values = new Dictionary<string, string>
            {
                { "to", contactOne.name },
                { "from", newMessage.currentUserId },
                { "content",  newMessage.content}

            };

            var content = new FormUrlEncodedContent(values);
            string remoteServer = String.Format("http://{0}/api/transfer",  contactOne.server);
            HttpClient client = new HttpClient();
            try {
                var response = await client.PostAsync(remoteServer, content);
                var responseString = await response.Content.ReadAsStringAsync();
            }
            catch {
                PostContact pc1 = new PostContact{name = contactOne.name, server = contactOne.server};
                q.updateContact(contactOne, pc1, msg);
                return Ok();
            }
            PostContact pc = new PostContact{name = contactOne.name, server = contactOne.server};
            q.updateContact(contactOne, pc, msg);
            return Ok();
        }


        string toSendInbox = db.InboxParticipants.Where(u => u.UserId == newMessage.id).FirstOrDefault().inboxUID;

        Inbox contactTwo = q.getContactByName(newMessage.currentUserId, toSendInbox);

        PostContact pcOne = new PostContact{name = contactOne.name, server = contactOne.server};
        q.updateContact(contactOne, pcOne, msg);
        if (contactTwo != null) {
            PostContact pcTwo = new PostContact{name = contactTwo.name, server = contactTwo.server};
            q.updateContact(contactTwo, pcTwo, msg);
        }
    }

    return Ok();

    }
}