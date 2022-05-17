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
    public ActionResult Index(RegisterModel newUser)
    {
        q.addNewUser(newUser);
 
        return View();
    }
}