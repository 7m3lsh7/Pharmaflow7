using System.ComponentModel.DataAnnotations;

namespace Pharmaflow7.Models
{
    public class ShipmentViewModel
    {
        
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string Destination { get; set; }

        public string DistributorId { get; set; } // اختياري

        public double LocationLat { get; set; }
        public double LocationLng { get; set; }

        // حقول العرض أو التي تُعين يدويًا (غير مطلوبة)
        public string Status { get; set; } // يُعين يدويًا في الكود
        public List<Product> Products { get; set; } // يُعين في الكود
        public IList<ApplicationUser> Distributors { get; set; } // يُعين في الكود
        public string ProductName { get; set; } // للعرض فقط
        public string CurrentLocation { get; set; } // للعرض فقط
        public string DistributorName { get; set; } // للعرض فقط
    }
}
