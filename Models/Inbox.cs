using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace chatWhatsappServer.Models
{
    public class Inbox
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get;set;}

        [StringLength(100)]
        public string inboxUID {get;set;}

        public string UserId {get;set;}
        
        [StringLength(50)]
        public Messages? last {get;set;}
        public string? lastdate {get;set;}
        
        public string name {get;set;}

        public string? server {get;set;}



    }
}