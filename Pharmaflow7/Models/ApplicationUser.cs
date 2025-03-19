using Microsoft.AspNetCore.Identity;

namespace Pharmaflow7.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Id { get; set; }

      
        public string RoleType { get; set; } = string.Empty; // مطلوب دائمًا، لكن نعطيه قيمة افتراضية
        public string? FullName { get; set; } // اختياري (للمستهلك)
        public string? Address { get; set; } // اختياري (للمستهلك)
        public string? CompanyName { get; set; } // اختياري (للشركة)
        public string? LicenseNumber { get; set; } // اختياري (للشركة)
        public string? ContactNumber { get; set; } // اختياري (للشركة والموزع)
        public string? DistributorName { get; set; } // اختياري (للموزع)
        public string? WarehouseAddress { get; set; } // اختياري (للموزع)
    }
}
