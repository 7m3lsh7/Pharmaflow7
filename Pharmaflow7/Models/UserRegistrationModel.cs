using System.ComponentModel.DataAnnotations;

namespace Pharmaflow7.Models
{
    public class UserRegistrationModel
    {
        public int Id { get; set; }
      
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "User type is required.")]
        public string RoleType { get; set; } // "consumer", "company", "distributor"

        // Consumer-specific fields
        public string FullName { get; set; } // مطلوب فقط لـ Consumer
        public string Address { get; set; }  // اختياري

        // Company-specific fields
        public string CompanyName { get; set; } // مطلوب فقط لـ Company
        public string LicenseNumber { get; set; } // مطلوب فقط لـ Company
        public string ContactNumber { get; set; } // مطلوب لـ Company و Distributor

        // Distributor-specific fields
        public string DistributorName { get; set; } // مطلوب فقط لـ Distributor
        public string WarehouseAddress { get; set; } // مطلوب فقط لـ Distributor
    

    }
}
