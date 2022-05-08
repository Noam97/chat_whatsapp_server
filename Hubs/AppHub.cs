using Microsoft.AspNetCore.SignalR;
using chatWhatsappServer.Models;
namespace chatWhatsappServer.Hubs {

    public class LogicHub : Hub {


        private ContactQueries q;
        private IConfiguration conf;

        public LogicHub(IConfiguration configuration)
        {
            conf = configuration;
            q = new ContactQueries(conf);

        }
        public async Task SendMessage(string sender, string receiver, string message) {

            q.createNewMessage(sender, receiver, "text", message);
            await Clients.Users(sender, receiver).SendAsync("ReceiveMessage", message);
        }


    }

}
