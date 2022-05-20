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
        List<Messages> msgs = q.getMessagesOf(currentUserId, contact.id);
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
        
        Inbox contact = q.getContactByName(newMessage.id, newMessage.inboxUID);

        PostContact pc = new PostContact{name = contact.name, server = contact.server,};

        q.updateContact(contact, pc, msg);
      
    }

    return Ok();

    }
}