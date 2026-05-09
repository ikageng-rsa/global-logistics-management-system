using GLMS.Web.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS.Web.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostUSD { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostZAR { get; set; }

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        // Navigation property
        public Contract? Contract { get; set; }
    }
}
