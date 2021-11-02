using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}