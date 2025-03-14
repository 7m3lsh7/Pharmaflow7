using System.ComponentModel.DataAnnotations;

namespace Pharmaflow7.Models
{
    public class UserRegistrationModel
    {
        public int Id { get; set; }

        [Required] 
        public string Email { get; set; }

        [Required] 
        public string CompanyName { get; set; }

        [Required]
        public string RoleType { get; set; } // Company أو Distributor

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
}
