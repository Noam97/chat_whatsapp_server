using Microsoft.AspNetCore.Mvc;
using chatWhatsappServer.DBModels;
using chatWhatsappServer.Models;


namespace chatWhatsappServer.Controllers; 

public class RegisterController : Controller
{
    private ContactQueries q;
    private IConfiguration conf;

    public RegisterController(IConfiguration configuration)
    {
        conf = configuration;
        q = new ContactQueries(conf);
    }
    public ActionResult Index()
    {
        return View();
    }
 
    [HttpPost]
    public ActionResult Index([FromBody]RegisterModel newUser)
    {
        if (!q.addNewUser(newUser)); {
            ViewBag.userExists = true;
            return View();

        }
        return RedirectToAction("", "Chat", new UserIdModel{Id = newUser.UserId});
    }
}