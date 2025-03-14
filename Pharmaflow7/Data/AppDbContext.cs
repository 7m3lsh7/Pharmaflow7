using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pharmaflow7.Models;

namespace Pharmaflow7.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        
              
        
        }
        public DbSet<UserRegistrationModel> userRegistrationModels { get; set; }
        public DbSet<LoginViewModel> loginViewModels { get; set; }
        public DbSet<DashboardViewModel> dashboardViewModels { get; set; }

    }
}
