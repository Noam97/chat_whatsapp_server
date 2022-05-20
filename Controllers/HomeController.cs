using Microsoft.AspNetCore.Mvc;
using chatWhatsappServer.DBModels;
using chatWhatsappServer.Models;

namespace chatWhatsappServer.Controllers; 

public class HomeController : Controller
{
    private IConfiguration conf;

    public HomeController(IConfiguration configuration)
    {
        conf = configuration;
    }
    public ActionResult Index()
    {
        return View();
    }
 
    [HttpPost]
    public ActionResult Index(HomeModel person)
    {
        string id = person.UserId;
        string password = person.Password;
        using ( var db = new EFContext(conf) )
        {
            if(db.Users.Where(x => x.Id == id && x.Password == password).FirstOrDefault() == null) {
                Console.WriteLine("not authorized");
                return View();
            }
            return RedirectToAction("", "Chat", new UserIdModel{Id = id});


        }
 
        return View();
    }
}