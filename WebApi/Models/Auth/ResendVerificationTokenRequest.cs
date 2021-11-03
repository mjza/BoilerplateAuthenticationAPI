using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class ResendVerificationTokenRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }
    }
}