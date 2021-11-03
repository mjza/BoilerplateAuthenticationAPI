using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }
    }
}