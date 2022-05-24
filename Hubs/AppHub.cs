using Microsoft.AspNetCore.SignalR;
using chatWhatsappServer.DBModels;
namespace chatWhatsappServer.Hubs {

    public class LogicHub : Hub {

    protected IHubContext<LogicHub> _context;

        private ContactQueries q;
        private IConfiguration conf;

        public LogicHub(IConfiguration configuration)
        {
            conf = configuration;
            q = new ContactQueries(conf);

        }
        public async Task SendMessage(string sender, string receiver, string inboxId, string message) {

            await Clients.All.SendAsync("ReceivedMessage", sender, receiver, inboxId, message);
        }


    }

}
