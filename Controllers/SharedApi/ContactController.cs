using Microsoft.AspNetCore.Mvc;
using chatWhatsappServer.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;
using chatWhatsappServer.Utils;
namespace chatWhatsappServer.Controllers.SharedApi 
{

    [Authorize]
    [Route("api/[controller]")]
    public class ContactsController: ControllerBase {

        private ContactQueries q;

        private IConfiguration conf;
        private SecUtils utils;
        public ContactsController(IConfiguration configuration)
        {
            conf = configuration;
            utils = new SecUtils(conf);
            q = new ContactQueries(conf);
        }

        [HttpGet]
        public ContentResult GetContacts() {
                string currentUserId = utils.ExtractUserIdFromJwt(HttpContext);
                List<Inbox> ContactList = q.getContactList(currentUserId);
                return Content(JsonSerializer.Serialize(ContactList), "application/json");
        }

        [Produces("application/json")]
        [HttpPost]
        public ActionResult createContact([FromBody] PostContact newContact) {
            string currentUserId = utils.ExtractUserIdFromJwt(HttpContext);
            q.addNewContact(newContact, currentUserId);
            return Ok();

        }

        [Route("{id}")]
        [HttpGet]
        public ContentResult GetContactById([FromRoute] string id) {
                string currentUserId = utils.ExtractUserIdFromJwt(HttpContext);
                List<Inbox> ContactList = q.getContactList(currentUserId);
                Inbox output = ContactList.Find(x => x.UserId == id);
                return Content(JsonSerializer.Serialize(output), "application/json");
        }

        [Route("{id}")]
        [HttpPut]
        public ActionResult UpdateContactById([FromRoute] string id, [FromBody] PostContact newContact) {
                string currentUserId = utils.ExtractUserIdFromJwt(HttpContext);
                List<Inbox> ContactList = q.getContactList(currentUserId);
                Inbox output = ContactList.Where(x => x.UserId == id).FirstOrDefault();
                if (output == null) {
                    return BadRequest("Contact does not exist");
                }
                if(q.updateContact(output, newContact) == true) {
                    Response.StatusCode = 204;
                    return Ok();
                }
                return BadRequest("Cannot update this contact");

        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult DeleteContactById([FromRoute] string id) {
                string currentUserId = utils.ExtractUserIdFromJwt(HttpContext);
                List<Inbox> ContactList = q.getContactList(currentUserId);
                Inbox output = ContactList.Where(x => x.UserId == id).FirstOrDefault();
                if (output == null) {
                    Response.StatusCode = 500;
                    return BadRequest("Contact does not exist");
                }
                if(q.deleteContact(output)) {
                    Response.StatusCode = 204;
                    return Ok();
                }
                Response.StatusCode = 500;
                return BadRequest("Contact does not exist");

        }

        [Route("{id}/messages")]
        [HttpGet]
        public ContentResult GetContactMessages([FromRoute] string id) {
                string currentUserId = utils.ExtractUserIdFromJwt(HttpContext);
                List<Messages> msgs = q.getMessagesOf(currentUserId, id);
                return Content(JsonSerializer.Serialize(msgs), "application/json");
        }

        [Route("{id}/messages")]
        [HttpPost]
        public IActionResult CreateNewMessage([FromRoute] string id, [FromBody] PostMessage newMessage) {
                string currentUserId = utils.ExtractUserIdFromJwt(HttpContext);
                if(q.createNewMessage(currentUserId, id, "text", newMessage.content)) {
                Response.StatusCode = 201;
                return Ok();
                }
                Response.StatusCode = 400;
                return BadRequest("Cannot send message");

        }

        [Route("{userId}/messages/{messageId}")]
        [HttpGet]
        public ContentResult GetSpecificMessage([FromRoute] string userId, [FromRoute] int messageId) {
                string currentUserId = utils.ExtractUserIdFromJwt(HttpContext);
                List<Messages> msgs = q.getMessagesOf(currentUserId, userId, messageId);
                if (msgs.First() != null )
                    return Content(JsonSerializer.Serialize(msgs[0]), "application/json");
                Response.StatusCode = 400;
                return Content("message has not found");
        }

        [Route("{userId}/messages/{messageId}")]
        [HttpPut]
        public IActionResult SetSpecificMessage([FromRoute] string userId, [FromRoute] int messageId, [FromBody] PostMessage content) {
                string currentUserId = utils.ExtractUserIdFromJwt(HttpContext);
                Messages msg = q.getMessagesOf(currentUserId, userId, messageId).FirstOrDefault();
                if (msg == null ) {
                    Response.StatusCode = 400;
                    return BadRequest("message has not found");
                }

                if(q.updateMessageOf(msg, content)) {
                    Response.StatusCode = 204;
                    return Ok();
                }
                Response.StatusCode = 400;
                return BadRequest("message cannot be updated");
        }

        [Route("{userId}/messages/{messageId}")]
        [HttpDelete]
        public IActionResult DeleteSpecificMessage([FromRoute] string userId, [FromRoute] int messageId) {
                string currentUserId = utils.ExtractUserIdFromJwt(HttpContext);
                Messages msg = q.getMessagesOf(currentUserId, userId, messageId).FirstOrDefault();
                if (msg == null ) {
                    Response.StatusCode = 400;
                    return BadRequest("message has not found");
                }

                if(q.DeleteSpecificMessage(msg)) {
                    Response.StatusCode = 204;
                    return Ok();
                }
                Response.StatusCode = 400;
                return BadRequest("message cannot be updated");
        }

    }

}
