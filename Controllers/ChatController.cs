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
    public IActionResult createContact([FromBody] PostContact newContact) {
        string currentUserId = newContact.currentUser;
        q.addNewContact(newContact, currentUserId);
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
    public IActionResult CreateNewMessage([FromBody] PostMessage newMessage) {

        using ( var db = new EFContext(conf) ) {

            if (newMessage.content == "") {
                return Ok();
            }

        Messages msg = new Messages{inboxUID = newMessage.inboxUID,
        UserId = newMessage.id, messageType = newMessage.messageType, content = newMessage.content, created = DateTime.UtcNow.ToString(), sent = newMessage.sent};
        db.Messages.Add(msg);
        db.SaveChanges();
        
        Inbox contactOne = q.getContactByName(newMessage.id, newMessage.inboxUID);


        string toSendInbox = db.InboxParticipants.Where(u => u.UserId == newMessage.id).FirstOrDefault().inboxUID;

        Inbox contactTwo = q.getContactByName(newMessage.currentUserId, toSendInbox);

        PostContact pcOne = new PostContact{name = contactOne.name, server = contactOne.server};
        PostContact pcTwo = new PostContact{name = contactTwo.name, server = contactTwo.server};


        q.updateContact(contactOne, pcOne, msg);
        q.updateContact(contactTwo, pcTwo, msg);
      
    }

    return Ok();

    }
}