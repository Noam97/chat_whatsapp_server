using Microsoft.AspNetCore.Mvc;
using chatWhatsappServer.DBModels;
using chatWhatsappServer.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;


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
                Console.WriteLine("Logged in");
                return View();
            }
            Console.WriteLine("Not Authorized");

        }
 
        return View();
    }
}