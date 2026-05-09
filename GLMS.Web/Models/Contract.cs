using GLMS.Web.Enums;
using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.Models
{
    public class Contract
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        [Required]
        public ServiceLevel ServiceLevel { get; set; } = ServiceLevel.Standard;

        // Stores the file path to the uploaded pdf agreement
        public string? AgreementFilePath { get; set; }

        // Navigation properties
        public Client? Client { get; set; }
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
