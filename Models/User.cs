using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace chatWhatsappServer.Models
{
    public class User
    {
        [Required(ErrorMessage="User Name should be specified")]
        [StringLength(50)]
        public string Id {get;set;}

        [StringLength(100)]
        public string? DisplayName {get;set;}

        [Required(ErrorMessage="Password must be specified")]
        [StringLength(50)]
        public string Password {get;set;}

        [StringLength(50)]
        public string? ProfileImage { get;set;}
    }
}