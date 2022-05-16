using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace chatWhatsappServer.DBModels
{
    public class InboxParticipants
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IPId {get;set;}

       [ForeignKey("User")]
        public string UserId {get;set;}

        public User? user {get;set;}

        [StringLength(100)]
        public string inboxUID {get;set;}
    }
}