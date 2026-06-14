using System.ComponentModel.DataAnnotations;

namespace GLMS.Api.DTOs
{
    public class UpdateUserRequest
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        public string? NewPassword { get; set; }
    }
}
