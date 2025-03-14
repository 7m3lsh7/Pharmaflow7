using Microsoft.AspNetCore.Identity;

namespace Pharmaflow7.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string CompanyName { get; set; } // اسم الشركة أو الموزع
        public string RoleType { get; set; } // "Company" أو "Distributor"
    }
}
