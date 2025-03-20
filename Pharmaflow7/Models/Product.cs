using System.ComponentModel.DataAnnotations;

namespace Pharmaflow7.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; } // معرف الشركة (مرتبط بـ ApplicationUser.Id)

       
    }
}
