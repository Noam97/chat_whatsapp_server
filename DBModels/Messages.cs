using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace chatWhatsappServer.DBModels
{
    public class Messages
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get;set;}

        [StringLength(100)]
        public string inboxUID {get;set;}

        public string UserId {get;set;}

        [Required(ErrorMessage="Message type cannot be empty")]
        [StringLength(50)]
        public string messageType {get;set;}    

        [Required(ErrorMessage="Message cannot be empty")]
        [StringLength(255)]
        public string content {get;set;}  

        [Required(ErrorMessage="Message must have created time")]
        public string created {get;set;}  

        public Boolean? sent {get;set;}  
        }
}
