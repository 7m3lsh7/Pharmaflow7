namespace Pharmaflow7.Models
{
    public class ShipmentViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Destination { get; set; }
        public string Status { get; set; }
        public string CurrentLocation { get; set; }
        public double LocationLat { get; set; }
        public double LocationLng { get; set; }
        public string? DistributorId { get; set; }
        public string DistributorName { get; set; }
        public IEnumerable<Product> Products { get; set; } // لعرض قائمة المنتجات في Create
        public IEnumerable<ApplicationUser> Distributors { get; set; } // لعرض قائمة الموزعين في Create
    }
}
