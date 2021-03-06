using System;
using System.Web;
using System.Web.Mvc;
using System.Linq; 
using Microsoft.EntityFrameworkCore;
using chatWhatsappServer.DBModels;
using chatWhatsappServer.Models;

public class PostContact
{
      public string id { get; set; }
      public string? name { get; set; }
      public string? server { get; set; }
      public string? currentUser { get; set; }

}

public class PostMessage
{
      public string? id { get; set; }

    public string? sender { get; set; }

      public string? content { get; set; }
      public string? inboxUID { get; set; }
      public string? messageType { get; set; }
      public string? created { get; set; }
      public string? currentUserId { get; set; }
      public bool? sent { get; set; }


}

class ContactQueries {
    private IConfiguration conf;
    public ContactQueries(IConfiguration configuration)
    {
        conf = configuration;
    }

    public List<Inbox> getContactList(String UserId) {
        using ( var db = new EFContext(conf) ) {

        InboxParticipants inboxParticipant = db.InboxParticipants.Where(user => user.UserId == UserId).FirstOrDefault();
        if (inboxParticipant == null) {
            return new List<Inbox>();
        }

        return (from users in db.Users
                join inboxpar in db.InboxParticipants on users.Id equals inboxpar.UserId
                join inbox in db.Inboxes on inboxpar.inboxUID equals inbox.inboxUID
                where inboxpar.IPId == inboxParticipant.IPId
                select new Inbox{
                                Id = inbox.Id,
                                UserId = inbox.UserId,
                                name = inbox.name,
                                server = Environment.MachineName,
                                inboxUID = inbox.inboxUID,
                                last = inbox.last,
                                lastdate = inbox.lastdate,
                                image = db.Users.Where(u => u.Id == inbox.UserId).First().ProfileImage
                            }).ToList();
        }

    }

    public Inbox getContactByName(string userId, string inboxUID) {
         using ( var db = new EFContext(conf) )
        {   
            string id = "";

            if (inboxUID == "") {
                var obj = db.InboxParticipants.Where(u => u.UserId == userId);
                if (obj == null) {
                    return null;
                }

                id = obj.FirstOrDefault().inboxUID;
                return db.Inboxes.Where(u => u.UserId == userId && u.inboxUID == id).FirstOrDefault();

            }
            return db.Inboxes.Where(u => u.UserId == userId && u.inboxUID == inboxUID).FirstOrDefault();
        }
    }

    public User getUser(string userId) {
         using ( var db = new EFContext(conf) )
        {   
            var contact = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            return contact;     
        }
    }

    public void addNewContact(PostContact newContact, string currentUserId) {
        using ( var db = new EFContext(conf) )
        {   
            var inbox = db.InboxParticipants.Where(u => u.UserId == currentUserId).FirstOrDefault();

            if(inbox != null && db.Inboxes.Where(i => i.inboxUID == inbox.inboxUID && i.UserId == newContact.id).FirstOrDefault() != null) {
                return;
            }

            if (db.Users.Where(u => u.Id == newContact.id).FirstOrDefault() == null) {
                addNewUser(new RegisterModel{UserId = newContact.id, DisplayName = newContact.id, Password = "", ProfileImage = ""});
            }
            
            InboxParticipants currentUserInbox = db.InboxParticipants.Where(i => i.UserId == currentUserId).FirstOrDefault();

            string uuid = (currentUserInbox == null) ? Guid.NewGuid().ToString("N") : currentUserInbox.inboxUID;

            // creates a new contact. if no inbox participant exists for current user, creates as well
            Inbox Contact = new Inbox{UserId = newContact.id, inboxUID = uuid, name = newContact.name, server = newContact.server};
            db.Inboxes.Add(Contact);
            db.SaveChanges();

            if(currentUserInbox == null) {

                InboxParticipants ip = new InboxParticipants{inboxUID = Contact.inboxUID, UserId = currentUserId};
                db.InboxParticipants.Add(ip);
                db.SaveChanges();
            }

            if(db.InboxParticipants.Where(u=> u.UserId == newContact.id).FirstOrDefault() == null) {
                InboxParticipants ip2 = new InboxParticipants{inboxUID = Guid.NewGuid().ToString("N"), UserId = newContact.id};
                db.InboxParticipants.Add(ip2);
                db.SaveChanges();
            }
        }
    }
    public bool addNewUser(RegisterModel newUser) {
        using ( var db = new EFContext(conf) )
        {   
            if(db.Users.Where(u => u.Id == newUser.UserId).FirstOrDefault() != null) {
                return false;
            }

            // creates a new user. 
           User new_user = new User{Id = newUser.UserId, Password = newUser.Password, DisplayName = newUser.DisplayName, ProfileImage = newUser.ProfileImage};
            db.Users.Add(new_user);
            db.SaveChanges();
            return true;
        }
    }

    public bool updateContact(Inbox contact , PostContact newAtts = null, Messages last = null) {
        try {
                using ( var db = new EFContext(conf) )
                {
                    if (newAtts.name == null) {
                        newAtts.name = contact.name;
                    }
                    if (newAtts.server == null) {
                        newAtts.server = contact.server;
                    }
                    if (last == null) {
                        last = contact.last;
                    }

                    var UpdatedContact = new Inbox{Id = contact.Id, name = newAtts.name, inboxUID = contact.inboxUID, server = newAtts.server, UserId = contact.UserId, last = last, lastdate = last.created};

                    var entity = db.Inboxes.Attach(UpdatedContact);
                    entity.State = EntityState.Modified;
                    db.SaveChanges();
                }
                return true;
        }
        catch (Exception ex){
            System.Console.Write(ex.ToString());
            return false;
        }
    }

    public bool deleteContact(Inbox contact) {
        //todo not working
        using ( var db = new EFContext(conf) ) {
            db.Inboxes.Remove(contact);
            db.SaveChanges();
        }
        return true;
    }

    public List<Messages> getChat(string currentUserId, string inputUserId) {
        using ( var db = new EFContext(conf) ) {

        InboxParticipants inboxParticipantOne = db.InboxParticipants.Where(user => user.UserId == currentUserId).FirstOrDefault();
        
        InboxParticipants inboxParticipantTwo = db.InboxParticipants.Where(user => user.UserId == inputUserId).FirstOrDefault();

        if (inboxParticipantOne == null && inboxParticipantTwo == null) {
            return new List<Messages>();
        }
        return db.Messages.Where(m => (m.inboxUID == inboxParticipantOne.inboxUID && m.UserId == inputUserId) || 
        (m.inboxUID == inboxParticipantTwo.inboxUID && m.UserId == currentUserId)).ToList();
        }

    }

    public List<Messages> getMessagesOf(string currentUserId, string inputUserId, int messageId = -1) {
        using ( var db = new EFContext(conf) ) {

        InboxParticipants inboxParticipant = db.InboxParticipants.Where(user => user.UserId == currentUserId).FirstOrDefault();
        if (inboxParticipant == null) {
            return new List<Messages>();
        }
        if(messageId > -1) {
            return db.Messages.Where(m => m.inboxUID == inboxParticipant.inboxUID && m.UserId == inputUserId && m.Id == messageId).ToList();

        }
        return db.Messages.Where(m => m.inboxUID == inboxParticipant.inboxUID && m.UserId == inputUserId).ToList();
        }

    }

    public bool createNewMessage(string currentUserId, string inputUserId, string messageType, string message) {

    using ( var db = new EFContext(conf) ) {

        InboxParticipants inboxParticipantFirst = db.InboxParticipants.Where(user => user.UserId == currentUserId).FirstOrDefault();
        
        InboxParticipants inboxParticipantSecond = db.InboxParticipants.Where(user => user.UserId == inputUserId).FirstOrDefault();

        if(inboxParticipantFirst == null || inboxParticipantSecond == null)
            return false;

        string inboxUIDFirst = inboxParticipantFirst.inboxUID;
        string inboxUIDSecond = inboxParticipantSecond.inboxUID;

        Inbox firstContact = getContactByName(inputUserId, inboxUIDFirst);
        Inbox secondContact = getContactByName(currentUserId, inboxUIDSecond);

        Messages msg = new Messages{inboxUID = firstContact.inboxUID, UserId = inputUserId,sender = currentUserId, messageType = messageType, content = message, created = DateTime.UtcNow.ToString(), sent = false};
        db.Messages.Add(msg);
        db.SaveChanges();
        


        PostContact pc1 = new PostContact{id = firstContact.UserId, name = firstContact.name, server = firstContact.server};

        PostContact pc2 = new PostContact{id = secondContact.UserId, name = secondContact.name, server = secondContact.server};


        updateContact(firstContact, pc1, msg);

        updateContact(secondContact, pc2, msg);
        
        return true;
    }
    }

    public bool updateMessageOf(Messages msg, PostMessage newAtts) {
    try {
            using ( var db = new EFContext(conf) )
            {
                if (newAtts.content == null) {
                    newAtts.content = msg.content;
                }

                var UpdatedMessage = new Messages{Id = msg.Id, inboxUID = msg.inboxUID, UserId = msg.UserId, messageType = msg.messageType, content = newAtts.content, created = msg.created, sent = msg.sent};

                var entity = db.Messages.Attach(UpdatedMessage);
                entity.State = EntityState.Modified;
                db.SaveChanges();
            }
            return true;
    }
    catch (Exception ex){
        System.Console.Write(ex.ToString());
        return false;
    }
}

    public bool DeleteSpecificMessage(Messages msg) {
        try {
        using ( var db = new EFContext(conf) ) {
            db.Messages.Remove(msg);
            db.SaveChanges();
        }
        return true;
        }
        catch (Exception ex) {
            return false;
        }
    }

}




