using Microsoft.AspNetCore.Identity;
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

            // العلاقة بين Product و ApplicationUser (الشركة المنتجة)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Company)
                .WithMany()
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ تغيير إلى Restrict بدلاً من NoAction

            // العلاقة بين Shipment و Product
            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ يسمح بحذف الشحنات عند حذف المنتج

            // العلاقة بين Shipment و Distributor
            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Distributor)
                .WithMany()
                .HasForeignKey(s => s.DistributorId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ تغيير إلى Restrict

            // العلاقة بين Shipment و ApplicationUser (الشركة)
            modelBuilder.Entity<Shipment>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(s => s.CompanyId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ تغيير إلى Restrict
        }
    


}
}