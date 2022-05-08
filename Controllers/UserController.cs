using Microsoft.AspNetCore.Mvc;
using chatWhatsappServer.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace chatWhatsappServer.Controllers.SharedApi 
{


    [Route("api/[controller]")]
    public class UsersController: ControllerBase {
        private IConfiguration conf;
        public UsersController(IConfiguration configuration)
        {
            conf = configuration;
        }

        [HttpPost]
        [Produces("application/json")]
        public IActionResult Post([FromBody] User user) {
            string userId = user.Id;
            string pwd = user.Password;
            using ( var db = new EFContext(conf) )
            {

                if(db.Users.Where(x => x.Id == userId && x.Password == pwd).FirstOrDefault() == null) {
                    Response.StatusCode = 401;
                    return BadRequest("Not Authenticated!");
                }
            }
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, conf["JWTParams:Subject"]),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, DateTime.UtcNow.ToString()),
            new Claim("UserId", userId)

        };

    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["JWTParams:SecretKey"]));

    var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
    
    var token = new JwtSecurityToken(
        conf["JWTParams:Issuer"],
        conf["JWTParams:Audience"],
        claims,
        expires: DateTime.UtcNow.AddMinutes(120),
        signingCredentials: mac);
    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }

    
}

}
