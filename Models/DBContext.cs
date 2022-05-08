
using Microsoft.EntityFrameworkCore;

namespace chatWhatsappServer.Models
{
    public class EFContext : DbContext
    {

        private IConfiguration conf;
        public EFContext(IConfiguration configuration)
        {
            conf = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
              optionsBuilder.UseMySql(conf["DBConnection"], MariaDbServerVersion.AutoDetect(conf["DBConnection"]));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.Entity<User>().HasKey(e => e.Id);
            modelBuilder.Entity<Messages>().HasKey(e => e.Id);
            modelBuilder.Entity<Inbox>().HasKey(e => e.Id);
            modelBuilder.Entity<InboxParticipants>().HasKey(e => e.IPId);

        }
        public DbSet<User> Users {get; set;}
        public DbSet<Inbox> Inboxes {get; set;}
        public DbSet<InboxParticipants> InboxParticipants {get; set;}
        public DbSet<Messages> Messages {get; set;}
    }
}