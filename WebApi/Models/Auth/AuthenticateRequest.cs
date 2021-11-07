using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Auth
{
    public class AuthenticateRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}