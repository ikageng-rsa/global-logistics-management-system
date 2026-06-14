using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace GLMS.Web.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ContactDetails { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Region { get; set; } = string.Empty;

        // Navigation properties
        public List<Contract> Contracts { get; set; } = new();
    }
}