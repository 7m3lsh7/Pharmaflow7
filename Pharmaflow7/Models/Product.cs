using System.ComponentModel.DataAnnotations;

namespace Pharmaflow7.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }


    }
}
