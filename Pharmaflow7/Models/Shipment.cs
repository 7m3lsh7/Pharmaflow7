using System.ComponentModel.DataAnnotations;

namespace Pharmaflow7.Models
{
    public class Shipment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

      

        [Required]
        public string Destination { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, InTransit, Delivered

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public int CompanyId { get; set; } // معرف الشركة

     
    }
}
