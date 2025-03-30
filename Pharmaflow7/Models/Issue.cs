namespace Pharmaflow7.Models
{
    public class Issue
    {
        public int Id { get; set; }
        public string CompanyId { get; set; }
        public int ProductId { get; set; }
        public string IssueType { get; set; } // مثل "Damaged", "Expired", ...
        public string ReportedById { get; set; }
        public DateTime ReportedDate { get; set; }
        public string Status { get; set; } // مثل "Open", "Resolved"

        public ApplicationUser Company { get; set; }
        public Product Product { get; set; }
        public ApplicationUser ReportedBy { get; set; }
    }
}