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

        // إزالة DbSets للـ ViewModels لأنها ليست كيانات مخزنة
        public DbSet<UserRegistrationModel> userRegistrationModels { get; set; }
        public DbSet<LoginViewModel> loginViewModels { get; set; }
        public DbSet<DashboardViewModel> dashboardViewModels { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Shipment> Shipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // لا حاجة لتكوين العلاقات لأننا لن نستخدم Foreign Keys
            // فقط تأكد من أن الأعمدة موجودة كحقول عادية في الجداول
        }
    }
}